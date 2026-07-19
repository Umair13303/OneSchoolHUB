using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Timetable;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/substitutions")]
[Authorize]
[Produces("application/json")]
public class SubstitutionController : ControllerBase
{
    private readonly ISubstitutionService _svc;
    public SubstitutionController(ISubstitutionService svc) => _svc = svc;

    /// <summary>Get all timetable slots for a specific date, showing existing substitutions.</summary>
    [HttpGet("day-slots")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetDaySlots([FromQuery] string date, [FromQuery] int? classId)
    {
        if (!DateOnly.TryParse(date, out var d))
            return BadRequest(new { error = "Invalid date. Use yyyy-MM-dd." });
        return Ok(await _svc.GetDaySlotsAsync(d, classId));
    }

    /// <summary>Get substitutions within a date range, optionally filtered by class.</summary>
    [HttpGet]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> GetByDateRange(
        [FromQuery] string from, [FromQuery] string to, [FromQuery] int? classId)
    {
        if (!DateOnly.TryParse(from, out var f) || !DateOnly.TryParse(to, out var t))
            return BadRequest(new { error = "Invalid date range. Use yyyy-MM-dd." });
        return Ok(await _svc.GetByDateRangeAsync(f, t, classId));
    }

    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Create([FromBody] CreateSubstitutionDto dto)
    {
        try
        {
            var result = await _svc.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(null, new { id = result.SubstitutionId }, result);
        }
        catch (ArgumentException ex)         { return BadRequest(new { error = ex.Message }); }
        catch (KeyNotFoundException ex)      { return NotFound(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubstitutionDto dto)
    {
        try   { return Ok(await _svc.UpdateAsync(id, dto, CurrentUserId())); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Delete(int id)
    {
        try   { await _svc.DeleteAsync(id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
