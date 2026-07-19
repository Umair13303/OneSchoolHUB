using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Bonus controller for Module 4 — Academic Years aren't called out in the
/// task doc's endpoint list, but every Class is scoped to one so we need
/// CRUD here. Reads are open to any authenticated user (dropdowns); writes
/// are SuperAdmin/Admin per the "Class & Subject Setup" access row.
/// </summary>
[ApiController]
[Route("api/academic-years")]
[Authorize]
[Produces("application/json")]
public class AcademicYearController : ControllerBase
{
    private readonly IAcademicYearService _service;

    public AcademicYearController(IAcademicYearService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var active = await _service.GetActiveAsync();
        return active is null ? NotFound() : Ok(active);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create([FromBody] CreateAcademicYearDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.AcademicYearId }, created);
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAcademicYearDto dto)
    {
        try
        {
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId());
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)        { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex){ return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:int}/set-active")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> SetActive(int id)
    {
        var ok = await _service.SetActiveAsync(id, CurrentUserId());
        return ok ? NoContent() : NotFound();
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

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
