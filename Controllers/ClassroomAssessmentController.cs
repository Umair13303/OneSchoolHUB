using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Assessment;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/assessments")]
[Authorize]
[Produces("application/json")]
public class ClassroomAssessmentController : ControllerBase
{
    private readonly IClassroomAssessmentService _service;

    public ClassroomAssessmentController(IClassroomAssessmentService service) => _service = service;

    private int CurrentUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id) ? id : 0;

    private bool IsTeacherOnly()
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return roles.Contains("teacher") && !roles.Contains("admin") && !roles.Contains("principal") && !roles.Contains("superadmin");
    }

    [HttpGet("result-lookups")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> GetLookups() => Ok(await _service.GetResultLookupsAsync());

    [HttpGet("roster")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> GetRoster([FromQuery] int classId, [FromQuery] int academicYearId)
    {
        if (classId <= 0 || academicYearId <= 0) return BadRequest(new { error = "classId and academicYearId are required." });
        return Ok(await _service.GetClassRosterAsync(classId, academicYearId));
    }

    [HttpPost("class")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> CreateAssessment([FromBody] CreateClassAssessmentDto dto)
    {
        try
        {
            var result = await _service.CreateAssessmentAsync(dto, CurrentUserId(), IsTeacherOnly());
            return Ok(result);
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("class")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> ListAssessments(
        [FromQuery] int? classId, [FromQuery] int? subjectId, [FromQuery] int? topicId,
        [FromQuery] string? from, [FromQuery] string? to)
    {
        DateOnly? fromD = DateOnly.TryParse(from, out var f) ? f : null;
        DateOnly? toD = DateOnly.TryParse(to, out var t) ? t : null;
        return Ok(await _service.ListAssessmentsAsync(classId, subjectId, topicId, fromD, toD));
    }

    [HttpGet("class/{id:int}")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> GetAssessment(int id)
    {
        var dto = await _service.GetAssessmentAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("class/{id:int}")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> UpdateAssessment(int id, [FromBody] UpdateClassAssessmentDto dto)
    {
        try
        {
            var ok = await _service.UpdateAssessmentAsync(id, dto, CurrentUserId(), IsTeacherOnly());
            return ok ? Ok(new { message = "Updated." }) : NotFound();
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("class/{id:int}/results")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> SaveResults(int id, [FromBody] BulkSaveClassAssessmentResultsDto dto)
    {
        try
        {
            return Ok(await _service.SaveAssessmentResultsAsync(id, dto, CurrentUserId(), IsTeacherOnly()));
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("sabaq/bulk")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> BulkSabaq([FromBody] BulkCreateStudentTopicPerformanceDto dto)
    {
        try
        {
            return Ok(await _service.BulkCreatePerformancesAsync(dto, CurrentUserId(), IsTeacherOnly()));
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("sabaq")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> ListSabaq(
        [FromQuery] int? topicId, [FromQuery] int? studentId, [FromQuery] int? classId,
        [FromQuery] string? from, [FromQuery] string? to)
    {
        DateOnly? fromD = DateOnly.TryParse(from, out var f) ? f : null;
        DateOnly? toD = DateOnly.TryParse(to, out var t) ? t : null;
        return Ok(await _service.ListPerformancesAsync(topicId, studentId, classId, fromD, toD));
    }

    [HttpGet("topics/{topicId:int}/summary")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> TopicSummary(int topicId)
        => Ok(await _service.GetTopicAssessmentSummaryAsync(topicId));

    [HttpGet("students/{studentId:int}/timeline")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> StudentTimeline(int studentId, [FromQuery] int? academicYearId, [FromQuery] int? subjectId)
        => Ok(await _service.GetStudentPerformanceTimelineAsync(studentId, academicYearId, subjectId));
}
