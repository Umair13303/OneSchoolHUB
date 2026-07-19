using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Controllers;

/// <summary>Reports (SRS §15) — Purchase, Sales, Inventory, Financial and Package reports.</summary>
[ApiController]
[Route("api/inventory/reports")]
[Authorize(Roles = "superadmin,admin,store_manager,accountant")]
[Produces("application/json")]
public class InventoryReportController : ControllerBase
{
    private readonly IInventoryReportService _reports;
    private readonly IStockService _stock;
    public InventoryReportController(IInventoryReportService reports, IStockService stock) { _reports = reports; _stock = stock; }

    // ── Purchase Reports ─────────────────────────────────────────────────────
    [HttpGet("purchase-summary")]
    public async Task<IActionResult> PurchaseSummary([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int? supplierId)
        => Ok(await _reports.GetPurchaseSummaryAsync(from, to, supplierId));

    [HttpGet("purchase-detail")]
    public async Task<IActionResult> PurchaseDetail([FromQuery] DateOnly? from, [FromQuery] DateOnly? to, [FromQuery] int? supplierId)
        => Ok(await _reports.GetPurchaseDetailAsync(from, to, supplierId));

    [HttpGet("supplier-purchase")]
    public async Task<IActionResult> SupplierPurchase([FromQuery] DateOnly? from, [FromQuery] DateOnly? to)
        => Ok(await _reports.GetSupplierPurchaseReportAsync(from, to));

    // ── Sales Reports ────────────────────────────────────────────────────────
    /// <summary>groupBy: Day | Month | Item | Student | User</summary>
    [HttpGet("sales")]
    public async Task<IActionResult> Sales([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string groupBy = "Day")
        => Ok(await _reports.GetSalesReportAsync(from, to, groupBy));

    // ── Inventory Reports ────────────────────────────────────────────────────
    [HttpGet("stock-balance")]
    public async Task<IActionResult> StockBalance([FromQuery] int? categoryId, [FromQuery] string? search)
        => Ok(await _stock.GetCurrentStockAsync(categoryId, search));

    [HttpGet("stock-ledger")]
    public async Task<IActionResult> StockLedger([FromQuery] int? itemId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _stock.GetLedgerAsync(itemId, from, to));

    [HttpGet("low-stock")]
    public async Task<IActionResult> LowStock() => Ok(await _stock.GetLowStockAsync());

    [HttpGet("out-of-stock")]
    public async Task<IActionResult> OutOfStock() => Ok(await _stock.GetOutOfStockAsync());

    [HttpGet("stock-valuation")]
    public async Task<IActionResult> StockValuation() => Ok(await _stock.GetStockValuationAsync());

    // ── Financial Reports ────────────────────────────────────────────────────
    [HttpGet("gross-profit")]
    public async Task<IActionResult> GrossProfit([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _reports.GetGrossProfitAsync(from, to));

    [HttpGet("profit-by-item")]
    public async Task<IActionResult> ProfitByItem([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _reports.GetProfitByItemAsync(from, to));

    [HttpGet("profit-by-category")]
    public async Task<IActionResult> ProfitByCategory([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _reports.GetProfitByCategoryAsync(from, to));

    // ── Package Reports ──────────────────────────────────────────────────────
    [HttpGet("package-sales")]
    public async Task<IActionResult> PackageSales([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _reports.GetPackageSalesAsync(from, to));

    [HttpGet("package-performance")]
    public async Task<IActionResult> PackagePerformance([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _reports.GetPackageSalesAsync(from, to));
}
