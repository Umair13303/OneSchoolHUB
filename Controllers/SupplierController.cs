using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Supplier Management (SRS §2).</summary>
[ApiController]
[Route("api/inventory/suppliers")]
[Authorize]
[Produces("application/json")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierService _service;
    public SupplierController(ISupplierService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] bool activeOnly = false)
        => Ok(await _service.GetAllAsync(search, activeOnly));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Create([FromBody] SaveSupplierDto dto)
    {
        var created = await _service.CreateAsync(dto, CurrentUserId());
        return CreatedAtAction(nameof(GetById), new { id = created.SupplierId }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Update(int id, [FromBody] SaveSupplierDto dto)
        => await _service.UpdateAsync(id, dto, CurrentUserId()) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Delete(int id)
    {
        try { return await _service.DeleteAsync(id, CurrentUserId()) ? NoContent() : NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
