using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Stock Management (SRS §9/§10) — current stock, ledger, adjustments (incl. damage/expired/physical), transfers.</summary>
[ApiController]
[Route("api/inventory/stock")]
[Authorize(Roles = "superadmin,admin,store_manager")]
[Produces("application/json")]
public class StockController : ControllerBase
{
    private readonly IStockService _service;
    public StockController(IStockService service) => _service = service;

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentStock([FromQuery] int? categoryId, [FromQuery] string? search)
        => Ok(await _service.GetCurrentStockAsync(categoryId, search));

    [HttpGet("low")]
    public async Task<IActionResult> GetLowStock() => Ok(await _service.GetLowStockAsync());

    [HttpGet("out-of-stock")]
    public async Task<IActionResult> GetOutOfStock() => Ok(await _service.GetOutOfStockAsync());

    [HttpGet("valuation")]
    public async Task<IActionResult> GetValuation() => Ok(await _service.GetStockValuationAsync());

    [HttpGet("ledger")]
    public async Task<IActionResult> GetLedger([FromQuery] int? itemId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _service.GetLedgerAsync(itemId, from, to));

    // ── Adjustments (Adjustment / Damage / Expired / Physical Verification) ──
    [HttpGet("adjustments")]
    public async Task<IActionResult> GetAdjustments() => Ok(await _service.GetAdjustmentsAsync());

    [HttpPost("adjustments")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> CreateAdjustment([FromBody] CreateStockAdjustmentDto dto)
    {
        try { return Ok(await _service.CreateAdjustmentAsync(dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    // ── Transfers ────────────────────────────────────────────────────────────
    [HttpGet("transfers")]
    public async Task<IActionResult> GetTransfers() => Ok(await _service.GetTransfersAsync());

    [HttpPost("transfers")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateStockTransferDto dto)
    {
        try { return Ok(await _service.CreateTransferAsync(dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
