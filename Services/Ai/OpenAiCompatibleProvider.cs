using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SchoolManagement.API.Services.Ai;

/// <summary>
/// OpenAI-compatible Chat Completions adapter (JSON object response).
/// API key stays server-side only.
/// </summary>
public class OpenAiCompatibleProvider : IAiProvider
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AiGenerationOptions _options;
    private readonly ILogger<OpenAiCompatibleProvider> _logger;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public OpenAiCompatibleProvider(
        IHttpClientFactory httpClientFactory,
        IOptions<AiGenerationOptions> options,
        ILogger<OpenAiCompatibleProvider> logger)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<string> CompleteJsonAsync(AiCompletionRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
            throw new InvalidOperationException("AI API key is not configured.");

        var client = _httpClientFactory.CreateClient("AiProvider");
        client.Timeout = TimeSpan.FromSeconds(Math.Clamp(_options.TimeoutSeconds, 10, 180));

        var baseUrl = _options.BaseUrl.TrimEnd('/');
        var url = $"{baseUrl}/chat/completions";

        var payload = new
        {
            model = _options.Model,
            temperature = 0.4,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = request.SystemPrompt },
                new { role = "user", content = request.UserPrompt }
            }
        };

        var json = JsonSerializer.Serialize(payload, JsonOpts);
        var maxRetries = Math.Clamp(_options.MaxRetries, 0, 2);
        Exception? last = null;

        for (var attempt = 0; attempt <= maxRetries; attempt++)
        {
            if (attempt > 0)
            {
                var delayMs = (int)(500 * Math.Pow(3, attempt - 1)); // 500, 1500
                await Task.Delay(delayMs, cancellationToken);
            }

            using var httpReq = new HttpRequestMessage(HttpMethod.Post, url);
            httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
            httpReq.Content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            try
            {
                response = await client.SendAsync(httpReq, cancellationToken);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException("AI provider timed out.", ex);
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(body);
                var content = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                if (string.IsNullOrWhiteSpace(content))
                    throw new InvalidOperationException("AI provider returned empty content.");

                return content;
            }

            var code = (int)response.StatusCode;
            var transient = response.StatusCode == HttpStatusCode.TooManyRequests
                            || code >= 500;

            _logger.LogWarning("AI provider HTTP {Status} (attempt {Attempt}/{Max}): {Body}",
                code, attempt + 1, maxRetries + 1, Truncate(body, 400));

            last = new InvalidOperationException($"AI provider error ({code}).");
            if (!transient || attempt == maxRetries)
                throw last;
        }

        throw last ?? new InvalidOperationException("AI provider failed.");
    }

    private static string Truncate(string s, int max) =>
        s.Length <= max ? s : s[..max] + "…";
}
