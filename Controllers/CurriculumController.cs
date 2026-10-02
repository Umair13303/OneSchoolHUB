using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Curriculum;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Curriculum / Course Content module.
/// Admin &amp; Principal manage Course Plans; Teachers update progress &amp; daily teaching via ClassSubject assignment.
/// </summary>
[ApiController]
[Route("api/curriculum")]
[Authorize]
[Produces("application/json")]
public class CurriculumController : ControllerBase
{
    private readonly ICurriculumService _service;

    public CurriculumController(ICurriculumService service) => _service = service;

    private int CurrentUserId() =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id) ? id : 0;

    private bool IsTeacherOnly()
    {
        var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return roles.Contains("teacher") && !roles.Contains("admin") && !roles.Contains("principal") && !roles.Contains("superadmin");
    }

    // ── Course Plans (Admin / Principal) ──────────────────────────────────────

    [HttpGet("plans")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> GetPlans(
        [FromQuery] int? academicYearId,
        [FromQuery] int? classId,
        [FromQuery] int? subjectId,
        [FromQuery] string? status)
    {
        var list = await _service.GetPlansAsync(academicYearId, classId, subjectId, status);
        return Ok(list);
    }

    [HttpGet("plans/{id:int}")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> GetPlan(int id)
    {
        if (IsTeacherOnly())
        {
            var ws = await _service.GetWorkspaceAsync(id, CurrentUserId(), requirePublished: true);
            return ws is null ? NotFound() : Ok(ws);
        }

        var dto = await _service.GetPlanAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("plans")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> CreatePlan([FromBody] CreateCoursePlanDto dto)
    {
        try
        {
            var result = await _service.CreatePlanAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetPlan), new { id = result.CoursePlanId }, result);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("plans/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UpdatePlan(int id, [FromBody] UpdateCoursePlanDto dto)
    {
        try
        {
            var ok = await _service.UpdatePlanAsync(id, dto, CurrentUserId());
            return ok ? Ok(new { message = "Updated." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("plans/{id:int}/publish")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Publish(int id)
    {
        try
        {
            var ok = await _service.PublishPlanAsync(id, CurrentUserId());
            return ok ? Ok(new { message = "Published." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("plans/{id:int}/archive")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> Archive(int id)
    {
        try
        {
            var ok = await _service.ArchivePlanAsync(id, CurrentUserId());
            return ok ? Ok(new { message = "Archived." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("plans/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> DeletePlan(int id)
    {
        var ok = await _service.SoftDeletePlanAsync(id, CurrentUserId());
        return ok ? Ok(new { message = "Deleted." }) : NotFound();
    }

    // ── Chapters ──────────────────────────────────────────────────────────────

    [HttpPost("chapters")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> CreateChapter([FromBody] CreateCourseChapterDto dto)
    {
        try { return Ok(await _service.CreateChapterAsync(dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("chapters/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UpdateChapter(int id, [FromBody] UpdateCourseChapterDto dto)
    {
        try
        {
            var ok = await _service.UpdateChapterAsync(id, dto, CurrentUserId());
            return ok ? Ok(new { message = "Updated." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("chapters/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> DeleteChapter(int id)
    {
        try
        {
            var ok = await _service.SoftDeleteChapterAsync(id, CurrentUserId());
            return ok ? Ok(new { message = "Deleted." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("plans/{planId:int}/chapters/reorder")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> ReorderChapters(int planId, [FromBody] ReorderDto dto)
    {
        try
        {
            await _service.ReorderChaptersAsync(planId, dto, CurrentUserId());
            return Ok(new { message = "Reordered." });
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Topics ────────────────────────────────────────────────────────────────

    [HttpPost("topics")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> CreateTopic([FromBody] CreateCourseTopicDto dto)
    {
        try { return Ok(await _service.CreateTopicAsync(dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("topics/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UpdateTopic(int id, [FromBody] UpdateCourseTopicDto dto)
    {
        try
        {
            var ok = await _service.UpdateTopicAsync(id, dto, CurrentUserId());
            return ok ? Ok(new { message = "Updated." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("topics/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> DeleteTopic(int id)
    {
        try
        {
            var ok = await _service.SoftDeleteTopicAsync(id, CurrentUserId());
            return ok ? Ok(new { message = "Deleted." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("chapters/{chapterId:int}/topics/reorder")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> ReorderTopics(int chapterId, [FromBody] ReorderDto dto)
    {
        try
        {
            await _service.ReorderTopicsAsync(chapterId, dto, CurrentUserId());
            return Ok(new { message = "Reordered." });
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("topics/{id:int}")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> GetTopic(int id)
    {
        var teacherId = IsTeacherOnly() ? CurrentUserId() : (int?)null;
        var dto = await _service.GetTopicDetailAsync(id, teacherId, requirePublished: IsTeacherOnly());
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPut("topics/{id:int}/progress")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> UpdateProgress(int id, [FromBody] UpdateTopicProgressDto dto)
    {
        try
        {
            await _service.UpdateTopicProgressAsync(id, dto, CurrentUserId());
            return Ok(new { message = "Progress updated." });
        }
        catch (UnauthorizedAccessException ex) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Materials ─────────────────────────────────────────────────────────────

    [HttpPost("materials")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> CreateMaterial([FromBody] CreateCourseMaterialDto dto)
    {
        try { return Ok(await _service.CreateMaterialAsync(dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("materials/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UpdateMaterial(int id, [FromBody] UpdateCourseMaterialDto dto)
    {
        try
        {
            var ok = await _service.UpdateMaterialAsync(id, dto, CurrentUserId());
            return ok ? Ok(new { message = "Updated." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("materials/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> DeleteMaterial(int id)
    {
        try
        {
            var ok = await _service.SoftDeleteMaterialAsync(id, CurrentUserId());
            return ok ? Ok(new { message = "Deleted." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Topic Activities ──────────────────────────────────────────────────────

    [HttpPost("activities")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> CreateActivity([FromBody] CreateCourseTopicActivityDto dto)
    {
        try { return Ok(await _service.CreateActivityAsync(dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("activities/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> UpdateActivity(int id, [FromBody] UpdateCourseTopicActivityDto dto)
    {
        try
        {
            var ok = await _service.UpdateActivityAsync(id, dto, CurrentUserId());
            return ok ? Ok(new { message = "Updated." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("activities/{id:int}")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        try
        {
            var ok = await _service.SoftDeleteActivityAsync(id, CurrentUserId());
            return ok ? Ok(new { message = "Deleted." }) : NotFound();
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("topics/{topicId:int}/activities/reorder")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> ReorderActivities(int topicId, [FromBody] ReorderDto dto)
    {
        try
        {
            await _service.ReorderActivitiesAsync(topicId, dto, CurrentUserId());
            return Ok(new { message = "Reordered." });
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Teacher workspace ─────────────────────────────────────────────────────

    [HttpGet("my-courses")]
    [Authorize(Roles = "teacher")]
    public async Task<IActionResult> MyCourses()
    {
        return Ok(await _service.GetMyCoursesAsync(CurrentUserId()));
    }

    [HttpGet("workspace/{planId:int}")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> Workspace(int planId)
    {
        var teacherId = IsTeacherOnly() ? CurrentUserId() : (int?)null;
        var dto = await _service.GetWorkspaceAsync(planId, teacherId, requirePublished: IsTeacherOnly());
        return dto is null ? NotFound() : Ok(dto);
    }

    // ── Daily Teaching ────────────────────────────────────────────────────────

    [HttpPost("teaching")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> CreateTeaching([FromBody] CreateTeachingLogDto dto)
    {
        try
        {
            var result = await _service.CreateTeachingLogAsync(dto, CurrentUserId());
            return Ok(result);
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Today's Learning / teaching diary for a class (parents included).</summary>
    [HttpGet("teaching")]
    [Authorize(Roles = "admin,principal,teacher,parent")]
    public async Task<IActionResult> GetTeaching(
        [FromQuery] int? classId,
        [FromQuery] string? date,
        [FromQuery] int? subjectId,
        [FromQuery] int? topicId,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        DateOnly? d = null, f = null, t = null;
        if (!string.IsNullOrWhiteSpace(date) && DateOnly.TryParse(date, out var pd)) d = pd;
        if (!string.IsNullOrWhiteSpace(from) && DateOnly.TryParse(from, out var pf)) f = pf;
        if (!string.IsNullOrWhiteSpace(to) && DateOnly.TryParse(to, out var pt)) t = pt;

        var list = await _service.GetTeachingLogsAsync(classId, d, subjectId, topicId, f, t);
        return Ok(list);
    }

    [HttpPost("teaching/{id:int}/homework")]
    [Authorize(Roles = "teacher,admin,principal")]
    public async Task<IActionResult> LinkHomework(int id, [FromBody] LinkHomeworkDto dto)
    {
        try
        {
            var result = await _service.LinkHomeworkAsync(id, dto, CurrentUserId());
            return Ok(result);
        }
        catch (UnauthorizedAccessException) { return Forbid(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    // ── Progress ──────────────────────────────────────────────────────────────

    [HttpGet("progress")]
    [Authorize(Roles = "admin,principal")]
    public async Task<IActionResult> ProgressSummaries(
        [FromQuery] int? academicYearId,
        [FromQuery] int? classId,
        [FromQuery] int? subjectId)
    {
        return Ok(await _service.GetProgressSummariesAsync(academicYearId, classId, subjectId));
    }

    [HttpGet("progress/{planId:int}")]
    [Authorize(Roles = "admin,principal,teacher")]
    public async Task<IActionResult> ProgressDetail(int planId)
    {
        if (IsTeacherOnly())
        {
            var ws = await _service.GetWorkspaceAsync(planId, CurrentUserId(), requirePublished: true);
            if (ws is null) return NotFound();
        }

        var dto = await _service.GetProgressDetailAsync(planId);
        return dto is null ? NotFound() : Ok(dto);
    }
}
