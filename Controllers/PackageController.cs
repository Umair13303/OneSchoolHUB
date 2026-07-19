using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Package / Bundle Management (SRS §6), e.g. "Class 1 Complete Book Set".</summary>
[ApiController]
[Route("api/inventory/packages")]
[Authorize]
[Produces("application/json")]
public class PackageController : ControllerBase
{
    private readonly IPackageService _service;
    public PackageController(IPackageService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false) => Ok(await _service.GetAllAsync(activeOnly));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Create([FromBody] CreatePackageDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.PackageId }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePackageDto dto)
    {
        try { return await _service.UpdateAsync(id, dto, CurrentUserId()) ? NoContent() : NotFound(); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Delete(int id)
    {
        try { return await _service.DeleteAsync(id, CurrentUserId()) ? NoContent() : NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
