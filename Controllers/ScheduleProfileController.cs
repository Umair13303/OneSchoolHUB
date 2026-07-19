using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Settings;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/schedule-profiles")]
[Authorize]
public class ScheduleProfileController : ControllerBase
{
    private readonly IScheduleProfileService _svc;
    public ScheduleProfileController(IScheduleProfileService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _svc.GetAsync(id));

    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Create([FromBody] CreateScheduleProfileDto dto) =>
        Ok(await _svc.CreateAsync(dto));

    [HttpPost("{id}/copy")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Copy(int id, [FromBody] CreateScheduleProfileDto dto) =>
        Ok(await _svc.CopyAsync(id, dto.Name));

    [HttpPatch("{id}/rename")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Rename(int id, [FromBody] RenameScheduleProfileDto dto) =>
        Ok(await _svc.RenameAsync(id, dto));

    [HttpPut("{id}/days")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UpdateDays(int id, [FromBody] BulkUpdateDayScheduleDto dto) =>
        Ok(await _svc.UpdateDaysAsync(id, dto.Days));

    [HttpPost("{id}/activate")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Activate(int id) => Ok(await _svc.ActivateAsync(id));

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Delete(int id)
    {
        try { await _svc.DeleteAsync(id); return NoContent(); }
        catch (InvalidOperationException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("sync-periods")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> SyncPeriods() =>
        Ok(await _svc.SyncPeriodsFromActiveProfileAsync());

    [HttpGet("active-periods-per-day")]
    public async Task<IActionResult> GetActivePeriodsPerDay() =>
        Ok(await _svc.GetActiveProfilePeriodsPerDayAsync());

    [HttpGet("{id}/periods")]
    public async Task<IActionResult> GetPeriods(int id) =>
        Ok(await _svc.GetPeriodsAsync(id));

    [HttpPut("{id}/periods")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> SavePeriods(int id, [FromBody] SaveProfilePeriodsDto dto) =>
        Ok(await _svc.SavePeriodsAsync(id, dto.Periods));
}
