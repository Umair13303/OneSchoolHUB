using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Homework;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module 7 — Homework / Student Diary.
///
///   POST   /api/homework                            → assign homework (teacher)
///   GET    /api/homework?classId=&amp;date=&amp;subjectId=  → diary for a class
///   GET    /api/homework/{id}                       → single homework detail
///   PUT    /api/homework/{id}                       → update homework
///   DELETE /api/homework/{id}                       → soft delete
///   GET    /api/homework/student/{studentId}?from=&amp;to= → student's diary
///   POST   /api/homework/{id}/submit                → student submits
///   GET    /api/homework/{id}/submissions           → all submissions (teacher view)
///   GET    /api/homework/{id}/submissions/{studentId} → one student's submission
///   PUT    /api/homework/submissions/{id}/review    → mark Reviewed
///
/// Access:
///   Assign / edit / review → teacher (owns the homework), superadmin, admin
///   View diary             → all roles except staff
///   Submit                 → student (via parent/self in future; no role restriction here)
/// </summary>
[ApiController]
[Route("api/homework")]
[Authorize]
[Produces("application/json")]
public class HomeworkController : ControllerBase
{
    private readonly IHomeworkService _service;
    private readonly IParentAccessService _parentAccess;
    public HomeworkController(IHomeworkService service, IParentAccessService parentAccess)
    {
        _service = service;
        _parentAccess = parentAccess;
    }

    // ── Assign homework ───────────────────────────────────────────────────────

    /// <summary>Assign new homework. TeacherId is taken from the JWT token.</summary>
    [HttpPost]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> Create([FromBody] CreateHomeworkDto dto)
    {
        try
        {
            var result = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = result.HomeworkId }, result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>Get homework by ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Get homework diary for a class.
    /// Required: classId. Optional: date (yyyy-MM-dd), subjectId.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetForClass(
        [FromQuery] int classId,
        [FromQuery] string? date,
        [FromQuery] int? subjectId)
    {
        if (classId <= 0)
            return BadRequest(new { error = "classId is required." });

        DateOnly? parsedDate = null;
        if (date is not null && !DateOnly.TryParse(date, out var d))
            return BadRequest(new { error = "date must be in yyyy-MM-dd format." });
        else if (date is not null)
            parsedDate = DateOnly.Parse(date);

        if (User.IsInRole("parent") && !await _parentAccess.HasChildInClassAsync(CurrentUserId(), classId))
            return Forbid();

        return Ok(await _service.GetForClassAsync(classId, parsedDate, subjectId));
    }

    /// <summary>Get all homework assigned to a student's enrolled classes. Optional: from, to (yyyy-MM-dd).</summary>
    [HttpGet("student/{studentId:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetForStudent(
        int studentId,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        if (User.IsInRole("parent") && !await _parentAccess.IsGuardianOfAsync(CurrentUserId(), studentId))
            return Forbid();

        return Ok(await _service.GetForStudentAsync(studentId, ParseDate(from), ParseDate(to)));
    }

    // ── Update / Delete ───────────────────────────────────────────────────────

    [HttpPut("{id:int}")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateHomeworkDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.SoftDeleteAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    // ── Submissions ───────────────────────────────────────────────────────────

    /// <summary>Student submits homework (optionally with a file).</summary>
    [HttpPost("{id:int}/submit")]
    [Authorize(Roles = "parent,student,teacher,admin,principal")]
    public async Task<IActionResult> Submit(int id, [FromBody] SubmitHomeworkDto dto)
    {
        if (User.IsInRole("parent") && !await _parentAccess.IsGuardianOfAsync(CurrentUserId(), dto.StudentId))
            return Forbid();

        try
        {
            var result = await _service.SubmitAsync(id, dto);
            return Ok(result);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Get all submissions for a homework assignment (teacher / admin view).</summary>
    [HttpGet("{id:int}/submissions")]
    [Authorize(Roles = "teacher,superadmin,admin,principal")]
    public async Task<IActionResult> GetSubmissions(int id)
        => Ok(await _service.GetSubmissionsAsync(id));

    /// <summary>Get a specific student's submission for a homework assignment.</summary>
    [HttpGet("{id:int}/submissions/{studentId:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetSubmissionByStudent(int id, int studentId)
    {
        if (User.IsInRole("parent") && !await _parentAccess.IsGuardianOfAsync(CurrentUserId(), studentId))
            return Forbid();

        var dto = await _service.GetSubmissionByStudentAsync(id, studentId);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Teacher marks a submission as Reviewed.</summary>
    [HttpPut("submissions/{submissionId:int}/review")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> Review(int submissionId, [FromBody] ReviewSubmissionDto dto)
    {
        try
        {
            var ok = await _service.ReviewAsync(submissionId, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private static DateOnly? ParseDate(string? value) =>
        DateOnly.TryParse(value, out var d) ? d : null;
}
