using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Browser uploads go here (same origin as the main API / CORS already works).
/// Hub forwards to the dedicated FileServer with X-Api-Key so live FE is not
/// blocked by FileServer's localhost-only CORS until that site is redeployed.
/// </summary>
[ApiController]
[Route("api/files")]
public class FilesProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<FilesProxyController> _logger;

    public FilesProxyController(
        IHttpClientFactory httpFactory,
        IConfiguration config,
        ILogger<FilesProxyController> logger)
    {
        _httpFactory = httpFactory;
        _config = config;
        _logger = logger;
    }

    [HttpPost("upload")]
    [Authorize]
    [RequestSizeLimit(52_428_800)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string entityType,
        [FromForm] int? entityId,
        [FromForm] string? schoolName,
        [FromForm] string? label,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "file is required and must not be empty." });
        if (string.IsNullOrWhiteSpace(entityType))
            return BadRequest(new { error = "entityType is required." });

        var (client, baseUrl) = CreateFileServerClient();
        if (client is null || string.IsNullOrWhiteSpace(baseUrl))
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "FileServer is not configured." });

        using var form = new MultipartFormDataContent();
        var streamContent = new StreamContent(file.OpenReadStream());
        if (!string.IsNullOrWhiteSpace(file.ContentType))
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        form.Add(streamContent, "file", file.FileName);
        form.Add(new StringContent(entityType), "entityType");
        if (entityId.HasValue) form.Add(new StringContent(entityId.Value.ToString()), "entityId");
        if (!string.IsNullOrWhiteSpace(schoolName)) form.Add(new StringContent(schoolName), "schoolName");
        if (!string.IsNullOrWhiteSpace(label)) form.Add(new StringContent(label), "label");

        using var res = await client.PostAsync($"{baseUrl}/api/files/upload", form, ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        if (!res.IsSuccessStatusCode)
        {
            _logger.LogWarning("FileServer upload failed: {Status} {Body}", (int)res.StatusCode, body);
            return new ContentResult
            {
                StatusCode = (int)res.StatusCode,
                Content = string.IsNullOrWhiteSpace(body) ? "{\"error\":\"Upload failed.\"}" : body,
                ContentType = "application/json"
            };
        }

        // Rewrite fileUrl to this Hub proxy so the browser stays on one origin.
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("fileId", out var idEl) && idEl.TryGetInt32(out var fileId))
            {
                var hubBase = $"{Request.Scheme}://{Request.Host}";
                var rewritten = System.Text.Json.JsonSerializer.Serialize(new
                {
                    fileId,
                    originalName = doc.RootElement.TryGetProperty("originalName", out var n) ? n.GetString() : file.FileName,
                    fileUrl = $"{hubBase}/api/files/{fileId}",
                    fileType = doc.RootElement.TryGetProperty("fileType", out var t) ? t.GetString() : file.ContentType,
                    sizeBytes = doc.RootElement.TryGetProperty("sizeBytes", out var s) && s.TryGetInt64(out var sb) ? sb : file.Length
                });
                return Content(rewritten, "application/json");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not rewrite FileServer upload response");
        }

        return Content(body, "application/json");
    }

    /// <summary>
    /// Stream a file for &lt;img src&gt; / downloads. Uses FileServer ApiKey server-side
    /// so the browser does not need a Bearer token on the image request.
    /// </summary>
    [HttpGet("{fileId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(int fileId, CancellationToken ct)
    {
        var (client, baseUrl) = CreateFileServerClient();
        if (client is null || string.IsNullOrWhiteSpace(baseUrl))
            return NotFound();

        using var res = await client.GetAsync($"{baseUrl}/api/files/{fileId}", ct);
        if (!res.IsSuccessStatusCode)
            return StatusCode((int)res.StatusCode);

        var contentType = res.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
        // Buffer fully — disposing the HttpResponseMessage must not close the body mid-stream.
        var bytes = await res.Content.ReadAsByteArrayAsync(ct);
        var fileName = res.Content.Headers.ContentDisposition?.FileName?.Trim('"');
        if (!string.IsNullOrWhiteSpace(fileName))
            return File(bytes, contentType, fileName);
        return File(bytes, contentType);
    }

    [HttpGet("{fileId:int}/info")]
    [Authorize]
    public async Task<IActionResult> Info(int fileId, CancellationToken ct)
    {
        var (client, baseUrl) = CreateFileServerClient();
        if (client is null || string.IsNullOrWhiteSpace(baseUrl))
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { error = "FileServer is not configured." });

        using var res = await client.GetAsync($"{baseUrl}/api/files/{fileId}/info", ct);
        var body = await res.Content.ReadAsStringAsync(ct);
        return Content(body, "application/json");
    }

    private (HttpClient? client, string? baseUrl) CreateFileServerClient()
    {
        var baseUrl = (_config["FileServer:BaseUrl"] ?? string.Empty).TrimEnd('/');
        var apiKey = _config["FileServer:ApiKey"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
            return (null, null);

        var client = _httpFactory.CreateClient("FileServer");
        client.DefaultRequestHeaders.Remove("X-Api-Key");
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        return (client, baseUrl);
    }
}
