using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
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
    private readonly IClassService _classes;
    private readonly IPeriodService _periods;
    private readonly IPdfReportService _pdf;
    private readonly AppDbContext _db;

    public TimetableController(
        ITimetableService service,
        IParentAccessService parentAccess,
        IClassService classes,
        IPeriodService periods,
        IPdfReportService pdf,
        AppDbContext db)
    {
        _service = service;
        _parentAccess = parentAccess;
        _classes = classes;
        _periods = periods;
        _pdf = pdf;
        _db = db;
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
    {
        // Teachers may only load their own schedule (prevents IDOR).
        if (User.IsInRole("teacher") && teacherId != CurrentUserId())
            return Forbid();

        return Ok(await _service.GetForTeacherAsync(teacherId, academicYearId));
    }

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

    /// <summary>
    /// Landscape class timetable PDF (school/campus header + printed-by footer).
    /// Avoids browser print chrome (title / URL / date headers).
    /// </summary>
    [HttpGet("pdf")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    [Produces("application/pdf")]
    public async Task<IActionResult> DownloadPdf(
        [FromQuery] int classId,
        [FromQuery] int? dayOfWeek,
        [FromQuery] int? academicYearId)
    {
        if (classId <= 0)
            return BadRequest(new { error = "classId query parameter is required." });

        if (User.IsInRole("parent") && !await _parentAccess.HasChildInClassAsync(CurrentUserId(), classId))
            return Forbid();

        var cls = await _classes.GetByIdAsync(classId);
        if (cls is null) return NotFound(new { error = "Class not found." });

        var entries = await _service.GetForClassAsync(classId, dayOfWeek, academicYearId);
        var periods = (await _periods.GetAllAsync())
            .OrderBy(p => p.StartTime)
            .ToList();

        // Prefer periods from schedule when master list is empty
        if (periods.Count == 0)
        {
            periods = entries
                .GroupBy(e => e.PeriodNo)
                .Select(g => g.First())
                .OrderBy(e => e.StartTime)
                .Select(e => new DTOs.Timetable.PeriodDto
                {
                    PeriodId = e.PeriodId,
                    PeriodNo = e.PeriodNo,
                    PeriodName = e.PeriodName,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    IsBreak = e.IsBreak
                })
                .ToList();
        }

        var days = dayOfWeek is >= 1 and <= 7
            ? new List<int> { dayOfWeek.Value }
            : entries.Select(e => e.DayOfWeek).Where(d => d is >= 1 and <= 7).Distinct().OrderBy(d => d).ToList();
        if (days.Count == 0)
            days = new List<int> { 1, 2, 3, 4, 5, 6 };

        var printedBy = await _db.Users.AsNoTracking()
            .Where(u => u.UserId == CurrentUserId())
            .Select(u => u.FullName)
            .FirstOrDefaultAsync() ?? "User";

        var model = new ClassTimetablePdfModel
        {
            SchoolName = cls.InstituteName?.Trim().Length > 0 ? cls.InstituteName! : "School",
            CampusName = string.IsNullOrWhiteSpace(cls.CampusName) ? null : cls.CampusName,
            ClassLabel = string.IsNullOrWhiteSpace(cls.Section)
                ? cls.ClassName
                : $"{cls.ClassName} – {cls.Section}",
            PrintedBy = printedBy,
            PrintedOn = DateTime.Now,
            Days = days,
            Periods = periods.Select(p =>
            {
                var row = new ClassTimetablePdfPeriod
                {
                    PeriodNo = p.PeriodNo,
                    StartTime = p.StartTime.ToString("HH\\:mm"),
                    EndTime = p.EndTime.ToString("HH\\:mm"),
                    IsBreak = p.IsBreak,
                    Cells = new Dictionary<int, ClassTimetablePdfCell?>()
                };
                foreach (var day in days)
                {
                    if (p.IsBreak)
                    {
                        row.Cells[day] = null;
                        continue;
                    }
                    var hit = entries.FirstOrDefault(e => e.PeriodNo == p.PeriodNo && e.DayOfWeek == day && !e.IsBreak);
                    row.Cells[day] = hit is null
                        ? null
                        : new ClassTimetablePdfCell
                        {
                            SubjectName = hit.SubjectName,
                            TeacherName = hit.TeacherName
                        };
                }
                return row;
            }).ToList()
        };

        var bytes = _pdf.ClassTimetablePdf(model);
        var safeClass = string.Concat(model.ClassLabel.Split(Path.GetInvalidFileNameChars()));
        var fileName = $"Timetable-{safeClass}.pdf";
        return File(bytes, "application/pdf", fileName);
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
