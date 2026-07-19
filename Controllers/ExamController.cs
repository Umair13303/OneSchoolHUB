using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Exam;
using SchoolManagement.API.Models;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module – Exam Management (Pakistan Educational Pattern, Class 1–12)
///
/// PAPER endpoints:
///   POST   /api/exam/papers                              → create paper (admin/teacher)
///   GET    /api/exam/papers                              → list papers (filtered)
///   GET    /api/exam/papers/{id}                         → paper detail + sections + schedule
///   PUT    /api/exam/papers/{id}                         → update paper
///   DELETE /api/exam/papers/{id}                         → soft delete
///   POST   /api/exam/papers/{id}/publish                 → publish draft
///
/// SCHEDULE endpoints:
///   POST   /api/exam/schedules                           → create schedule
///   GET    /api/exam/schedules                           → get schedules (filter by class/date)
///   PUT    /api/exam/schedules/{id}                      → update schedule
///   DELETE /api/exam/schedules/{id}                      → delete schedule
///
/// RESULT endpoints:
///   POST   /api/exam/results                             → enter single result
///   POST   /api/exam/results/bulk                        → bulk result entry (whole class)
///   POST   /api/exam/results/{paperId}/recalculate-ranks → recalculate class ranks
///   GET    /api/exam/results/{paperId}/summary           → class result summary
///   GET    /api/exam/results/student/{studentId}         → student's all results
///   GET    /api/exam/results/student/{studentId}/card    → student result card
///
/// Access:
///   Create / edit paper, enter results → teacher, admin, superadmin, principal
///   View papers / results              → all roles except staff
///   Publish / delete                   → admin, superadmin, principal
/// </summary>
[ApiController]
[Route("api/exam")]
[Authorize]
[Produces("application/json")]
public class ExamController : ControllerBase
{
    private readonly IExamService _service;
    public ExamController(IExamService service) => _service = service;

    // ═══════════════════════════════════════════════════════════════════════════
    // PAPERS
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Create a new exam paper.
    /// Sections are optional at creation – can be added via update.
    /// For Class 9–12 (Matric/Inter), the recommended section structure is:
    ///   Section A – Objective (MCQs)
    ///   Section B – Short Questions
    ///   Section C – Long Questions
    /// </summary>
    [HttpPost("papers")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> CreatePaper([FromBody] CreateExamPaperDto dto)
    {
        try
        {
            var result = await _service.CreatePaperAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetPaper), new { id = result.ExamPaperId }, result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>
    /// List exam papers with optional filters.
    /// </summary>
    [HttpGet("papers")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetPapers(
        [FromQuery] int?      classId        = null,
        [FromQuery] int?      subjectId      = null,
        [FromQuery] int?      academicYearId = null,
        [FromQuery] ExamType? examType       = null)
    {
        var list = await _service.GetPapersAsync(classId, subjectId, academicYearId, examType);
        return Ok(list);
    }

    /// <summary>Get full paper detail including sections and schedules.</summary>
    [HttpGet("papers/{id:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetPaper(int id)
    {
        var paper = await _service.GetPaperByIdAsync(id);
        return paper is null ? NotFound() : Ok(paper);
    }

    /// <summary>Update paper metadata and/or sections. Locked papers cannot be edited.</summary>
    [HttpPut("papers/{id:int}")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> UpdatePaper(int id, [FromBody] UpdateExamPaperDto dto)
    {
        try
        {
            var ok = await _service.UpdatePaperAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Soft-delete a paper (only if no results have been entered).</summary>
    [HttpDelete("papers/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> DeletePaper(int id)
    {
        try
        {
            var ok = await _service.DeletePaperAsync(id, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Publish a draft paper so teachers can enter results.</summary>
    [HttpPost("papers/{id:int}/publish")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> PublishPaper(int id)
    {
        try
        {
            var result = await _service.PublishPaperAsync(id, CurrentUserId());
            return Ok(result);
        }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SCHEDULES
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>Schedule an exam paper for a date/time/room.</summary>
    [HttpPost("schedules")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> CreateSchedule([FromBody] CreateExamScheduleDto dto)
    {
        try
        {
            var result = await _service.CreateScheduleAsync(dto, CurrentUserId());
            return Created($"api/exam/schedules/{result.ExamScheduleId}", result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Get exam schedule with optional filters (classId, date range).</summary>
    [HttpGet("schedules")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetSchedules(
        [FromQuery] int?      classId  = null,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate   = null)
    {
        var list = await _service.GetSchedulesAsync(classId, fromDate, toDate);
        return Ok(list);
    }

    /// <summary>Update schedule (reschedule, change room, assign invigilator, cancel).</summary>
    [HttpPut("schedules/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateExamScheduleDto dto)
    {
        var ok = await _service.UpdateScheduleAsync(id, dto, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Remove a schedule entry.</summary>
    [HttpDelete("schedules/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var ok = await _service.DeleteScheduleAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // RESULTS
    // ═══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Enter result for a single student.
    /// Marks the paper as locked and computes grade/percentage automatically.
    /// </summary>
    [HttpPost("results")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> EnterResult([FromBody] EnterExamResultDto dto)
    {
        try
        {
            var result = await _service.EnterResultAsync(dto, CurrentUserId());
            return Created($"api/exam/results/{result.ExamResultId}", result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>
    /// Bulk result entry for an entire class at once.
    /// After save, class ranks are automatically recalculated.
    /// </summary>
    [HttpPost("results/bulk")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> BulkEnterResults([FromBody] BulkEnterExamResultDto dto)
    {
        try
        {
            await _service.BulkEnterResultsAsync(dto, CurrentUserId());
            return Ok(new { message = "Results saved and ranks recalculated." });
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Manually trigger rank recalculation for a paper.</summary>
    [HttpPost("results/{paperId:int}/recalculate-ranks")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> RecalculateRanks(int paperId)
    {
        await _service.RecalculateRanksAsync(paperId);
        return Ok(new { message = "Ranks recalculated." });
    }

    /// <summary>
    /// Class result summary: pass/fail counts, highest/lowest/average marks,
    /// pass percentage, and individual student results with ranks.
    /// </summary>
    [HttpGet("results/{paperId:int}/summary")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> GetClassResultSummary(int paperId)
    {
        try
        {
            var summary = await _service.GetClassResultSummaryAsync(paperId);
            return Ok(summary);
        }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
    }

    /// <summary>All results for a student (optionally filtered by academic year).</summary>
    [HttpGet("results/student/{studentId:int}")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetStudentResults(int studentId, [FromQuery] int? academicYearId = null)
    {
        var results = await _service.GetStudentResultsAsync(studentId, academicYearId);
        return Ok(results);
    }

    /// <summary>
    /// Student result card for a specific academic year and exam type
    /// (e.g. all Final Exam results for Class 9A in year 2025-26).
    /// </summary>
    [HttpGet("results/student/{studentId:int}/card")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,parent")]
    public async Task<IActionResult> GetStudentResultCard(
        int studentId,
        [FromQuery] int      academicYearId = 0,
        [FromQuery] ExamType examType       = ExamType.FinalExam)
    {
        if (academicYearId == 0) return BadRequest(new { error = "academicYearId is required." });
        try
        {
            var card = await _service.GetStudentResultCardAsync(studentId, academicYearId, examType);
            return Ok(card);
        }
        catch (ArgumentException ex) { return NotFound(new { error = ex.Message }); }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // QUESTIONS
    // Endpoints:
    //   GET    /api/exam/papers/{paperId}/questions          → list questions
    //   POST   /api/exam/questions                           → add single question
    //   PUT    /api/exam/questions/{id}                      → update question
    //   DELETE /api/exam/questions/{id}                      → delete question
    //   POST   /api/exam/papers/{paperId}/questions/save-all → bulk save (replace all)
    // ═══════════════════════════════════════════════════════════════════════════

    [HttpGet("papers/{paperId}/questions")]
    [Authorize(Roles = "superadmin,admin,principal,teacher")]
    public async Task<IActionResult> GetQuestions(int paperId)
    {
        var list = await _service.GetQuestionsAsync(paperId);
        return Ok(list);
    }

    [HttpPost("questions")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> AddQuestion([FromBody] CreateExamQuestionDto dto)
    {
        try
        {
            var result = await _service.AddQuestionAsync(dto, CurrentUserId());
            return Ok(result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("questions/{id}")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> UpdateQuestion(int id, [FromBody] UpdateExamQuestionDto dto)
    {
        try
        {
            var ok = await _service.UpdateQuestionAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("questions/{id}")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> DeleteQuestion(int id)
    {
        var ok = await _service.DeleteQuestionAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("papers/{paperId}/questions/save-all")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> SaveAllQuestions(int paperId, [FromBody] SavePaperQuestionsDto dto)
    {
        dto.ExamPaperId = paperId;
        try
        {
            var list = await _service.SavePaperQuestionsAsync(dto, CurrentUserId());
            return Ok(list);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private int CurrentUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User ID not found in token."));
}
