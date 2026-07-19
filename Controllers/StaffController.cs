using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Staff;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize(Roles = "superadmin,admin,principal")]
[Produces("application/json")]
public class StaffController : ControllerBase
{
    private readonly IStaffService _service;
    public StaffController(IStaffService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? department,
        [FromQuery] string? status)
        => Ok(await _service.GetAllAsync(search, department, status));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Create([FromBody] CreateStaffDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.StaffId }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStaffDto dto)
    {
        var ok = await _service.UpdateAsync(id, dto, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.DeleteAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/create-login")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> CreateLogin(int id, [FromBody] CreateStaffLoginDto dto)
    {
        try
        {
            var result = await _service.CreateLoginAsync(id, dto, CurrentUserId());
            return Ok(result);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    /// <summary>List documents (degree certs, CNIC, etc) attached to this staff member.</summary>
    [HttpGet("{id:int}/documents")]
    public async Task<IActionResult> GetDocuments(int id)
        => Ok(await _service.GetDocumentsAsync(id));

    /// <summary>Tag a file already uploaded to the FileServer as belonging to this staff member, under the given label.</summary>
    [HttpPost("{id:int}/documents")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> TagDocument(int id, [FromBody] TagStaffDocumentDto dto)
    {
        try
        {
            var result = await _service.TagDocumentAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Remove a document from this staff member's list (soft-delete).</summary>
    [HttpDelete("{id:int}/documents/{fileId:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> RemoveDocument(int id, int fileId)
    {
        var ok = await _service.RemoveDocumentAsync(id, fileId);
        return ok ? NoContent() : NotFound();
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
