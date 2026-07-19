using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Item categories, units of measure and tax rates — shared lookup data for the Inventory & POS module.</summary>
[ApiController]
[Route("api/inventory")]
[Authorize]
[Produces("application/json")]
public class InventoryMasterController : ControllerBase
{
    private readonly IInventoryMasterService _service;
    public InventoryMasterController(IInventoryMasterService service) => _service = service;

    // ── Categories ───────────────────────────────────────────────────────────
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories() => Ok(await _service.GetCategoriesAsync());

    [HttpPost("categories")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> CreateCategory([FromBody] SaveItemCategoryDto dto)
        => Ok(await _service.CreateCategoryAsync(dto, CurrentUserId()));

    [HttpPut("categories/{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] SaveItemCategoryDto dto)
        => await _service.UpdateCategoryAsync(id, dto, CurrentUserId()) ? NoContent() : NotFound();

    [HttpDelete("categories/{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try { return await _service.DeleteCategoryAsync(id, CurrentUserId()) ? NoContent() : NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    // ── Units ────────────────────────────────────────────────────────────────
    [HttpGet("units")]
    public async Task<IActionResult> GetUnits() => Ok(await _service.GetUnitsAsync());

    [HttpPost("units")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> CreateUnit([FromBody] SaveUnitDto dto)
        => Ok(await _service.CreateUnitAsync(dto, CurrentUserId()));

    [HttpPut("units/{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> UpdateUnit(int id, [FromBody] SaveUnitDto dto)
        => await _service.UpdateUnitAsync(id, dto, CurrentUserId()) ? NoContent() : NotFound();

    [HttpDelete("units/{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> DeleteUnit(int id)
    {
        try { return await _service.DeleteUnitAsync(id, CurrentUserId()) ? NoContent() : NotFound(); }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
    }

    // ── Tax ──────────────────────────────────────────────────────────────────
    [HttpGet("taxes")]
    public async Task<IActionResult> GetTaxes() => Ok(await _service.GetTaxesAsync());

    [HttpPost("taxes")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> CreateTax([FromBody] SaveTaxDto dto)
        => Ok(await _service.CreateTaxAsync(dto, CurrentUserId()));

    [HttpPut("taxes/{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> UpdateTax(int id, [FromBody] SaveTaxDto dto)
        => await _service.UpdateTaxAsync(id, dto, CurrentUserId()) ? NoContent() : NotFound();

    [HttpDelete("taxes/{id:int}")]
    [Authorize(Roles = "admin,store_manager")]
    public async Task<IActionResult> DeleteTax(int id)
        => await _service.DeleteTaxAsync(id, CurrentUserId()) ? NoContent() : NotFound();

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
