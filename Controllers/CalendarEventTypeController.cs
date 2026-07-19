using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/calendar-event-types")]
[Authorize]
[Produces("application/json")]
public class CalendarEventTypeController : ControllerBase
{
    private readonly ICalendarEventTypeService _svc;
    public CalendarEventTypeController(ICalendarEventTypeService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Create([FromBody] CreateCalendarEventTypeDto dto)
    {
        try
        {
            var result = await _svc.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetAll), result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCalendarEventTypeDto dto)
    {
        try
        {
            var ok = await _svc.UpdateAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _svc.DeleteAsync(id, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
