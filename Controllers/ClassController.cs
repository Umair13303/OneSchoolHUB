using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module 4 — Classes. Routes match the task doc:
///   GET/POST/PUT  /api/classes
///   POST          /api/classes/{id}/assign-teacher
/// </summary>
[ApiController]
[Route("api/classes")]
[Authorize]
[Produces("application/json")]
public class ClassController : ControllerBase
{
    private readonly IClassService _service;

    public ClassController(IClassService service) => _service = service;

    /// <summary>List classes. Optionally filter by academic year.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? academicYearId)
        => Ok(await _service.GetAllAsync(academicYearId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>List subjects-with-teacher assigned to a class.</summary>
    [HttpGet("{id:int}/subjects")]
    public async Task<IActionResult> GetSubjects(int id)
        => Ok(await _service.GetSubjectsForClassAsync(id));

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateClassDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.ClassId }, created);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateClassDto dto)
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
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Assign (or replace) the teacher for one subject in this class.</summary>
    [HttpPost("{id:int}/assign-teacher")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> AssignTeacher(int id, [FromBody] AssignTeacherDto dto)
    {
        try
        {
            var result = await _service.AssignTeacherAsync(id, dto, CurrentUserId());
            return Ok(result);
        }
        catch (ArgumentException ex){ return BadRequest(new { error = ex.Message }); }
    }

    /// <summary>Get all class-subject assignments for a specific teacher.</summary>
    [HttpGet("teacher-assignments/{teacherId:int}")]
    public async Task<IActionResult> GetTeacherAssignments(int teacherId)
        => Ok(await _service.GetAssignmentsByTeacherAsync(teacherId));

    /// <summary>Remove a subject from a class.</summary>
    [HttpDelete("{id:int}/subjects/{classSubjectId:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> RemoveSubject(int id, int classSubjectId)
    {
        var ok = await _service.RemoveSubjectAsync(id, classSubjectId, CurrentUserId());
        return ok ? NoContent() : NotFound();
    }

    /// <summary>Remove teacher from a class subject (keep the subject, just unassign teacher).</summary>
    [HttpPatch("{id:int}/subjects/{classSubjectId:int}/unassign-teacher")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UnassignTeacher(int id, int classSubjectId)
    {
        var result = await _service.UnassignTeacherAsync(id, classSubjectId, CurrentUserId());
        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>Toggle active/inactive for a class subject.</summary>
    [HttpPatch("{id:int}/subjects/{classSubjectId:int}/toggle-status")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ToggleSubjectStatus(int id, int classSubjectId)
    {
        var result = await _service.ToggleSubjectStatusAsync(id, classSubjectId, CurrentUserId());
        return result is null ? NotFound() : Ok(result);
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
