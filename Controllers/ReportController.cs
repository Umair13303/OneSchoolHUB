using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module 8 — Reports &amp; Filters.
///
///   GET /api/reports/attendance/daily?classId=&amp;date=
///   GET /api/reports/attendance/monthly?classId=&amp;month=&amp;year=
///   GET /api/reports/attendance/student/{studentId}?from=&amp;to=
///   GET /api/reports/homework?classId=&amp;from=&amp;to=
///   GET /api/reports/enrollment?academicYearId=
///   GET /api/reports/teacher-workload?academicYearId=
///
/// Access: superadmin, admin, principal for all reports.
///         teacher can view attendance and homework reports.
///         parent can view their own student's attendance report only.
/// </summary>
[ApiController]
[Route("api/reports")]
[Authorize]
[Produces("application/json")]
public class ReportController : ControllerBase
{
    private readonly IReportService _service;
    private readonly IPdfReportService _pdf;
    public ReportController(IReportService service, IPdfReportService pdf)
    {
        _service = service;
        _pdf = pdf;
    }

    // ── Attendance ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Daily attendance roll for a class — all periods, all students.
    /// Required: classId, date (yyyy-MM-dd).
    /// </summary>
    [HttpGet("attendance/daily")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> AttendanceDaily(
        [FromQuery] int classId,
        [FromQuery] string date)
    {
        if (classId <= 0)
            return BadRequest(new { error = "classId is required." });
        if (!DateOnly.TryParse(date, out var parsedDate))
            return BadRequest(new { error = "date must be in yyyy-MM-dd format." });

        var report = await _service.AttendanceDailyAsync(classId, parsedDate);
        return report is null ? NotFound(new { error = $"Class {classId} not found." }) : Ok(report);
    }

    /// <summary>
    /// Monthly attendance summary for all students in a class.
    /// Required: classId, month (1–12), year.
    /// </summary>
    [HttpGet("attendance/monthly")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> AttendanceMonthly(
        [FromQuery] int classId,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        if (classId <= 0)
            return BadRequest(new { error = "classId is required." });
        if (month < 1 || month > 12)
            return BadRequest(new { error = "month must be between 1 and 12." });
        if (year < 2000)
            return BadRequest(new { error = "year is invalid." });

        var report = await _service.AttendanceMonthlyAsync(classId, month, year);
        return report is null ? NotFound(new { error = $"Class {classId} not found." }) : Ok(report);
    }

    /// <summary>
    /// Full attendance report for a single student.
    /// Optional: from, to (yyyy-MM-dd).
    /// </summary>
    [HttpGet("attendance/student/{studentId:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> StudentAttendance(
        int studentId,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        var report = await _service.StudentAttendanceAsync(studentId, ParseDate(from), ParseDate(to));
        return report is null ? NotFound(new { error = $"Student {studentId} not found." }) : Ok(report);
    }

    // ── Homework ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Homework completion report for a class — submission rates per assignment.
    /// Required: classId. Optional: from, to (yyyy-MM-dd).
    /// </summary>
    [HttpGet("homework")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> Homework(
        [FromQuery] int classId,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        if (classId <= 0)
            return BadRequest(new { error = "classId is required." });

        return Ok(await _service.HomeworkAsync(classId, ParseDate(from), ParseDate(to)));
    }

    // ── Enrollment ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Class-wise enrollment report for an academic year.
    /// Required: academicYearId.
    /// Returns all classes with their student lists and counts.
    /// </summary>
    [HttpGet("enrollment")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> Enrollment([FromQuery] int academicYearId)
    {
        if (academicYearId <= 0)
            return BadRequest(new { error = "academicYearId is required." });

        var report = await _service.ClassEnrollmentAsync(academicYearId);
        return report is null
            ? NotFound(new { error = $"Academic year {academicYearId} not found." })
            : Ok(report);
    }

    // ── Teacher Workload ───────────────────────────────────────────────────────

    /// <summary>
    /// Teacher workload report — periods per week and homework assigned per teacher.
    /// Required: academicYearId.
    /// </summary>
    [HttpGet("teacher-workload")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> TeacherWorkload([FromQuery] int academicYearId)
    {
        if (academicYearId <= 0)
            return BadRequest(new { error = "academicYearId is required." });

        var report = await _service.TeacherWorkloadAsync(academicYearId);
        return report is null
            ? NotFound(new { error = $"Academic year {academicYearId} not found." })
            : Ok(report);
    }

    // ── PDF Endpoints ──────────────────────────────────────────────────────────

    [HttpGet("attendance/daily/pdf")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> AttendanceDailyPdf([FromQuery] int classId, [FromQuery] string date)
    {
        if (classId <= 0) return BadRequest(new { error = "classId is required." });
        if (!DateOnly.TryParse(date, out var parsedDate)) return BadRequest(new { error = "date must be in yyyy-MM-dd format." });
        var report = await _service.AttendanceDailyAsync(classId, parsedDate);
        if (report is null) return NotFound();
        var bytes = _pdf.DailyAttendancePdf(report);
        return File(bytes, "application/pdf", $"attendance-daily-{date}.pdf");
    }

    [HttpGet("attendance/monthly/pdf")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> AttendanceMonthlyPdf([FromQuery] int classId, [FromQuery] int month, [FromQuery] int year)
    {
        if (classId <= 0) return BadRequest(new { error = "classId is required." });
        var report = await _service.AttendanceMonthlyAsync(classId, month, year);
        if (report is null) return NotFound();
        var bytes = _pdf.MonthlyAttendancePdf(report);
        return File(bytes, "application/pdf", $"attendance-monthly-{year}-{month:D2}.pdf");
    }

    [HttpGet("attendance/student/{studentId:int}/pdf")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> StudentAttendancePdf(int studentId, [FromQuery] string? from, [FromQuery] string? to)
    {
        var report = await _service.StudentAttendanceAsync(studentId, ParseDate(from), ParseDate(to));
        if (report is null) return NotFound();
        var bytes = _pdf.StudentAttendancePdf(report);
        return File(bytes, "application/pdf", $"student-attendance-{studentId}.pdf");
    }

    [HttpGet("homework/pdf")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> HomeworkPdf([FromQuery] int classId, [FromQuery] string? from, [FromQuery] string? to)
    {
        if (classId <= 0) return BadRequest(new { error = "classId is required." });
        var report = await _service.HomeworkAsync(classId, ParseDate(from), ParseDate(to));
        var bytes = _pdf.HomeworkPdf(report);
        return File(bytes, "application/pdf", $"homework-report-class-{classId}.pdf");
    }

    [HttpGet("enrollment/pdf")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> EnrollmentPdf([FromQuery] int academicYearId)
    {
        if (academicYearId <= 0) return BadRequest(new { error = "academicYearId is required." });
        var report = await _service.ClassEnrollmentAsync(academicYearId);
        if (report is null) return NotFound();
        var bytes = _pdf.EnrollmentPdf(report);
        return File(bytes, "application/pdf", $"enrollment-{academicYearId}.pdf");
    }

    [HttpGet("teacher-workload/pdf")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> TeacherWorkloadPdf([FromQuery] int academicYearId)
    {
        if (academicYearId <= 0) return BadRequest(new { error = "academicYearId is required." });
        var report = await _service.TeacherWorkloadAsync(academicYearId);
        if (report is null) return NotFound();
        var bytes = _pdf.TeacherWorkloadPdf(report);
        return File(bytes, "application/pdf", $"teacher-workload-{academicYearId}.pdf");
    }

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static DateOnly? ParseDate(string? value) =>
        DateOnly.TryParse(value, out var d) ? d : null;
}
