using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Attendance;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module 6 — Attendance (per period).
///
///   POST  /api/attendance/mark         → mark single student
///   POST  /api/attendance/bulk         → mark whole class for one period
///   PUT   /api/attendance/{id}         → update a record
///   GET   /api/attendance?classId=&amp;date=&amp;periodId=   → class roll for a date
///   GET   /api/attendance/student/{id}?from=&amp;to=       → student history
///   GET   /api/attendance/student/{id}/summary?from=&amp;to= → student summary
///   GET   /api/attendance/class/{id}/summary?from=&amp;to=  → class summary
///
/// Access:
///   Mark  → teacher only
///   View  → superadmin, admin, principal, teacher, parent
///          (parent restriction to own child is enforced in a future phase)
/// </summary>
[ApiController]
[Route("api/attendance")]
[Authorize]
[Produces("application/json")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _service;
    private readonly IParentAccessService _parentAccess;

    public AttendanceController(IAttendanceService service, IParentAccessService parentAccess)
    {
        _service = service;
        _parentAccess = parentAccess;
    }

    /// <summary>Mark attendance for a single student in one period.</summary>
    [HttpPost("mark")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> MarkSingle([FromBody] MarkAttendanceDto dto)
    {
        try
        {
            var record = await _service.MarkSingleAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetForClass),
                new { classId = record.ClassId, date = record.Date.ToString("yyyy-MM-dd") },
                record);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Mark attendance for all students in a class for one period at once.</summary>
    [HttpPost("bulk")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> BulkMark([FromBody] BulkMarkAttendanceDto dto)
    {
        try
        {
            var records = await _service.BulkMarkAsync(dto, CurrentUserId());
            return Ok(records);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Update an existing attendance record (change status or remarks).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendanceDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>
    /// Get attendance roll for a class on a given date.
    /// Required: classId, date (yyyy-MM-dd). Optional: periodId.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetForClass(
        [FromQuery] int classId,
        [FromQuery] string date,
        [FromQuery] int? periodId)
    {
        if (classId <= 0)
            return BadRequest(new { error = "classId is required." });
        if (!DateOnly.TryParse(date, out var parsedDate))
            return BadRequest(new { error = "date must be in yyyy-MM-dd format." });

        if (User.IsInRole("parent") && !await _parentAccess.HasChildInClassAsync(CurrentUserId(), classId))
            return Forbid();

        return Ok(await _service.GetForClassAsync(classId, parsedDate, periodId));
    }

    /// <summary>Get full attendance history for a student. Optional date range: from, to (yyyy-MM-dd).</summary>
    [HttpGet("student/{studentId:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetForStudent(
        int studentId,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        if (User.IsInRole("parent") && !await _parentAccess.IsGuardianOfAsync(CurrentUserId(), studentId))
            return Forbid();

        DateOnly? fromDate = ParseDate(from);
        DateOnly? toDate   = ParseDate(to);
        return Ok(await _service.GetForStudentAsync(studentId, fromDate, toDate));
    }

    /// <summary>Get attendance summary (present/absent/late counts + %) for one student.</summary>
    [HttpGet("student/{studentId:int}/summary")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetStudentSummary(
        int studentId,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        if (User.IsInRole("parent") && !await _parentAccess.IsGuardianOfAsync(CurrentUserId(), studentId))
            return Forbid();

        var summary = await _service.GetSummaryAsync(studentId, ParseDate(from), ParseDate(to));
        return summary is null ? NotFound() : Ok(summary);
    }

    /// <summary>Get attendance summary for every student in a class over a date range.</summary>
    [HttpGet("class/{classId:int}/summary")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> GetClassSummary(
        int classId,
        [FromQuery] string from,
        [FromQuery] string to)
    {
        if (!DateOnly.TryParse(from, out var fromDate) || !DateOnly.TryParse(to, out var toDate))
            return BadRequest(new { error = "from and to must be in yyyy-MM-dd format." });

        return Ok(await _service.GetClassSummaryAsync(classId, fromDate, toDate));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private static DateOnly? ParseDate(string? value) =>
        DateOnly.TryParse(value, out var d) ? d : null;
}
