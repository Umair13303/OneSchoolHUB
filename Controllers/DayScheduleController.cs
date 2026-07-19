using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Settings;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/day-schedule")]
[Authorize]
public class DayScheduleController : ControllerBase
{
    private readonly IDayScheduleService _svc;
    public DayScheduleController(IDayScheduleService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _svc.GetAllAsync());

    [HttpPut]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> BulkUpdate([FromBody] BulkUpdateDayScheduleDto dto) =>
        Ok(await _svc.BulkUpdateAsync(dto.Days));
}
