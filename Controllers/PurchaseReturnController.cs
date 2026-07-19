using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Purchase Return (SRS §11) — returning purchased items to a supplier.</summary>
[ApiController]
[Route("api/inventory/purchase-returns")]
[Authorize(Roles = "superadmin,admin,store_manager")]
[Produces("application/json")]
public class PurchaseReturnController : ControllerBase
{
    private readonly IPurchaseReturnService _service;
    public PurchaseReturnController(IPurchaseReturnService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseReturnDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.PurchaseReturnId }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
