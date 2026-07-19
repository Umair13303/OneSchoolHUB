using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/academic-years/{yearId:int}/events")]
[Authorize]
[Produces("application/json")]
public class CalendarEventController : ControllerBase
{
    private readonly ICalendarEventService _svc;
    public CalendarEventController(ICalendarEventService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll(int yearId)
        => Ok(await _svc.GetByYearAsync(yearId));

    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Create(int yearId, [FromBody] CreateCalendarEventDto dto)
    {
        try
        {
            var result = await _svc.CreateAsync(yearId, dto, CurrentUserId());
            return CreatedAtAction(nameof(GetAll), new { yearId }, result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (ArgumentException ex)    { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{eventId:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Update(int yearId, int eventId, [FromBody] UpdateCalendarEventDto dto)
    {
        try
        {
            var ok = await _svc.UpdateAsync(eventId, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{eventId:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Delete(int yearId, int eventId)
    {
        var ok = await _svc.DeleteAsync(eventId, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
