using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Timetable;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module 5 — Timetable. Endpoint routes match the task doc:
///   GET    /api/timetable?classId=&dayOfWeek=
///   POST   /api/timetable
///   PUT    /api/timetable/{id}
///   GET    /api/timetable/teacher/{teacherId}
///
/// Access:
///   View  → superadmin, admin, principal, teacher, parent
///   Setup → superadmin, admin, principal (per the Module Access table)
/// </summary>
[ApiController]
[Route("api/timetable")]
[Authorize]
[Produces("application/json")]
public class TimetableController : ControllerBase
{
    private readonly ITimetableService _service;
    private readonly IParentAccessService _parentAccess;

    public TimetableController(ITimetableService service, IParentAccessService parentAccess)
    {
        _service = service;
        _parentAccess = parentAccess;
    }

    /// <summary>List a class's timetable. Required: <c>classId</c>. Optional: <c>dayOfWeek</c>, <c>academicYearId</c>.</summary>
    [HttpGet]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetForClass(
        [FromQuery] int classId,
        [FromQuery] int? dayOfWeek,
        [FromQuery] int? academicYearId)
    {
        if (classId <= 0)
            return BadRequest(new { error = "classId query parameter is required." });

        if (User.IsInRole("parent") && !await _parentAccess.HasChildInClassAsync(CurrentUserId(), classId))
            return Forbid();

        return Ok(await _service.GetForClassAsync(classId, dayOfWeek, academicYearId));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Teacher's own timetable, grouped by day-of-week.</summary>
    [HttpGet("teacher/{teacherId:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetForTeacher(
        int teacherId,
        [FromQuery] int? academicYearId)
        => Ok(await _service.GetForTeacherAsync(teacherId, academicYearId));

    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Create([FromBody] CreateTimetableEntryDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.TimetableId }, created);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTimetableEntryDto dto)
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
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.SoftDeleteAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
