using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Point of Sale (SRS §7) — search, checkout, hold/resume invoices, receipt data.</summary>
[ApiController]
[Route("api/pos")]
[Authorize(Roles = "superadmin,admin,store_manager,cashier")]
[Produces("application/json")]
public class PosController : ControllerBase
{
    private readonly IPosService _service;
    public PosController(IPosService service) => _service = service;

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term) => Ok(await _service.SearchAsync(term ?? string.Empty));

    [HttpGet("sales")]
    public async Task<IActionResult> GetAll([FromQuery] DateTime? from, [FromQuery] DateTime? to,
        [FromQuery] int? cashierId, [FromQuery] int? studentId, [FromQuery] string? status)
        => Ok(await _service.GetAllAsync(from, to, cashierId, studentId, status));

    [HttpGet("sales/held")]
    public async Task<IActionResult> GetHeld() => Ok(await _service.GetHeldInvoicesAsync());

    [HttpGet("sales/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _service.GetByIdAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>Checkout. Set Hold=true to park the cart (no stock/payment posted) or false to complete the sale.</summary>
    [HttpPost("sales")]
    [Authorize(Roles = "admin,store_manager,cashier")]
    public async Task<IActionResult> Create([FromBody] CreateSaleDto dto)
    {
        try
        {
            var created = await _service.CreateSaleAsync(dto, CurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.SalesId }, created);
        }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    /// <summary>Resume a held invoice and complete the sale (deducts stock, records payment).</summary>
    [HttpPost("sales/{id:int}/resume")]
    [Authorize(Roles = "admin,store_manager,cashier")]
    public async Task<IActionResult> Resume(int id, [FromBody] CompleteSaleDto dto)
    {
        try { return Ok(await _service.CompleteSaleAsync(id, dto, CurrentUserId())); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    [HttpPost("sales/{id:int}/void")]
    [Authorize(Roles = "admin,store_manager,cashier")]
    public async Task<IActionResult> VoidHeld(int id)
    {
        try { return await _service.VoidHeldInvoiceAsync(id, CurrentUserId()) ? NoContent() : NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
