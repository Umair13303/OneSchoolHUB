using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Timetable;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// CRUD for the school's daily period schedule (Module 5). Periods rarely
/// change after initial setup, so writes are SuperAdmin-only; reads are open
/// to any authenticated user (Timetable + Attendance + Homework UIs all need
/// the period list).
/// </summary>
[ApiController]
[Route("api/periods")]
[Authorize]
[Produces("application/json")]
public class PeriodController : ControllerBase
{
    private readonly IPeriodService _service;

    public PeriodController(IPeriodService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreatePeriodDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.PeriodId }, created);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePeriodDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _service.SoftDeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Replace the entire period schedule in one shot (used by Settings page).</summary>
    [HttpPost("bulk-replace")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> BulkReplace([FromBody] BulkReplacePeriodsDto dto)
    {
        try
        {
            var result = await _service.BulkReplaceAsync(dto.Periods, CurrentUserId());
            return Ok(result);
        }
        catch (FormatException ex) { return BadRequest(new { error = $"Invalid time format: {ex.Message}" }); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
