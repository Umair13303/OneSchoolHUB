using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Purchase Management (SRS §3) — posting a purchase increases stock and updates costing.</summary>
[ApiController]
[Route("api/inventory/purchases")]
[Authorize(Roles = "superadmin,admin,store_manager")]
[Produces("application/json")]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _service;
    public PurchaseController(IPurchaseService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int? supplierId)
        => Ok(await _service.GetAllAsync(from, to, supplierId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.PurchaseId }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Cancel(int id)
        => await _service.CancelAsync(id, CurrentUserId()) ? NoContent() : NotFound();

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
