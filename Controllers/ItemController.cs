using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Item Master (SRS §1) — create/edit/deactivate inventory items, barcode lookup, POS search.</summary>
[ApiController]
[Route("api/inventory/items")]
[Authorize]
[Produces("application/json")]
public class ItemController : ControllerBase
{
    private readonly IItemService _service;
    public ItemController(IItemService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] string? search, [FromQuery] bool activeOnly = false)
        => Ok(await _service.GetAllAsync(categoryId, search, activeOnly));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var dto = await _service.FindByBarcodeAsync(barcode);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Combined item+package search used by the POS screen's search/barcode box.</summary>
    [HttpGet("pos-search")]
    public async Task<IActionResult> PosSearch([FromQuery] string term) => Ok(await _service.SearchForPosAsync(term ?? string.Empty));

    [HttpPost]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Create([FromBody] CreateItemDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.ItemId }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItemDto dto)
    {
        try
        {
            // Only Administrator / Store Manager may change sale/wholesale/minimum-sale price (SRS §5).
            var canEditPrice = User.IsInRole("superadmin") || User.IsInRole("admin") || User.IsInRole("store_manager");
            var ok = await _service.UpdateAsync(id, dto, CurrentUserId(), canEditPrice);
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> Delete(int id)
    {
        try { return await _service.DeleteAsync(id, CurrentUserId()) ? NoContent() : NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPatch("{id:int}/toggle-status")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> ToggleStatus(int id)
        => await _service.ToggleStatusAsync(id, CurrentUserId()) ? NoContent() : NotFound();

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
