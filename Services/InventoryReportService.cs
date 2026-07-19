using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;

namespace SchoolManagement.API.Services;

/// <summary>Read-only reporting queries for SRS §15 (Purchase / Sales / Inventory / Financial / Package reports).</summary>
public interface IInventoryReportService
{
    Task<List<PurchaseSummaryRowDto>> GetPurchaseSummaryAsync(DateOnly? from, DateOnly? to, int? supplierId);
    Task<List<PurchaseDetailRowDto>> GetPurchaseDetailAsync(DateOnly? from, DateOnly? to, int? supplierId);
    Task<List<SalesSummaryRowDto>> GetSupplierPurchaseReportAsync(DateOnly? from, DateOnly? to);

    /// <summary>groupBy: Day | Month | Item | Student | User</summary>
    Task<List<SalesSummaryRowDto>> GetSalesReportAsync(DateTime? from, DateTime? to, string groupBy);

    Task<ProfitRowDto> GetGrossProfitAsync(DateTime? from, DateTime? to);
    Task<List<ProfitRowDto>> GetProfitByItemAsync(DateTime? from, DateTime? to);
    Task<List<ProfitRowDto>> GetProfitByCategoryAsync(DateTime? from, DateTime? to);

    Task<List<PackageSalesRowDto>> GetPackageSalesAsync(DateTime? from, DateTime? to);
}

public class InventoryReportService : IInventoryReportService
{
    private readonly AppDbContext _db;
    public InventoryReportService(AppDbContext db) => _db = db;

    // ── Purchase Reports ─────────────────────────────────────────────────────
    public async Task<List<PurchaseSummaryRowDto>> GetPurchaseSummaryAsync(DateOnly? from, DateOnly? to, int? supplierId)
    {
        var q = _db.PurchaseMasters.AsNoTracking().Include(p => p.Supplier)
            .Where(p => p.Status != "Cancelled").AsQueryable();
        if (from.HasValue) q = q.Where(p => p.PurchaseDate >= from.Value);
        if (to.HasValue) q = q.Where(p => p.PurchaseDate <= to.Value);
        if (supplierId.HasValue) q = q.Where(p => p.SupplierId == supplierId.Value);

        var list = await q.OrderByDescending(p => p.PurchaseDate).ToListAsync();
        return list.Select(p => new PurchaseSummaryRowDto
        {
            PurchaseId = p.PurchaseId,
            PurchaseNo = p.PurchaseNo,
            PurchaseDate = p.PurchaseDate,
            SupplierName = p.Supplier.SupplierName,
            GrossAmount = p.GrossAmount,
            DiscountAmount = p.DiscountAmount,
            TaxAmount = p.TaxAmount,
            NetAmount = p.NetAmount
        }).ToList();
    }

    public async Task<List<PurchaseDetailRowDto>> GetPurchaseDetailAsync(DateOnly? from, DateOnly? to, int? supplierId)
    {
        var q = _db.PurchaseDetails.AsNoTracking()
            .Include(d => d.Purchase).ThenInclude(p => p.Supplier)
            .Include(d => d.Item)
            .Where(d => d.Purchase.Status != "Cancelled")
            .AsQueryable();
        if (from.HasValue) q = q.Where(d => d.Purchase.PurchaseDate >= from.Value);
        if (to.HasValue) q = q.Where(d => d.Purchase.PurchaseDate <= to.Value);
        if (supplierId.HasValue) q = q.Where(d => d.Purchase.SupplierId == supplierId.Value);

        var list = await q.OrderByDescending(d => d.Purchase.PurchaseDate).ToListAsync();
        return list.Select(d => new PurchaseDetailRowDto
        {
            PurchaseDate = d.Purchase.PurchaseDate,
            PurchaseNo = d.Purchase.PurchaseNo,
            SupplierName = d.Purchase.Supplier.SupplierName,
            ItemName = d.Item.ItemName,
            Quantity = d.Quantity,
            PurchasePrice = d.PurchasePrice,
            NetAmount = d.NetAmount
        }).ToList();
    }

    public async Task<List<SalesSummaryRowDto>> GetSupplierPurchaseReportAsync(DateOnly? from, DateOnly? to)
    {
        var rows = await GetPurchaseSummaryAsync(from, to, null);
        return rows.GroupBy(r => r.SupplierName).Select(g => new SalesSummaryRowDto
        {
            Label = g.Key,
            Quantity = 0,
            GrossAmount = g.Sum(x => x.GrossAmount),
            DiscountAmount = g.Sum(x => x.DiscountAmount),
            TaxAmount = g.Sum(x => x.TaxAmount),
            NetAmount = g.Sum(x => x.NetAmount),
            InvoiceCount = g.Count()
        }).OrderByDescending(r => r.NetAmount).ToList();
    }

    // ── Sales Reports ────────────────────────────────────────────────────────
    public async Task<List<SalesSummaryRowDto>> GetSalesReportAsync(DateTime? from, DateTime? to, string groupBy)
    {
        var q = _db.SalesMasters.AsNoTracking()
            .Where(s => s.Status == "Completed" || s.Status == "PartiallyReturned" || s.Status == "Returned")
            .Include(s => s.Details).AsQueryable();
        if (from.HasValue) q = q.Where(s => s.SalesDate >= from.Value);
        if (to.HasValue) q = q.Where(s => s.SalesDate <= to.Value);

        var sales = await q.ToListAsync();

        string LabelFor(Models.SalesMaster s) => groupBy switch
        {
            "Month" => s.SalesDate.ToString("yyyy-MM"),
            "Day"   => s.SalesDate.ToString("yyyy-MM-dd"),
            _       => s.SalesId.ToString() // Item/Student/User grouped below at line level
        };

        if (groupBy is "Day" or "Month")
        {
            return sales.GroupBy(LabelFor).Select(g => new SalesSummaryRowDto
            {
                Label = g.Key,
                Quantity = g.Sum(s => s.Details.Sum(d => d.Quantity)),
                GrossAmount = g.Sum(s => s.GrossAmount),
                DiscountAmount = g.Sum(s => s.DiscountAmount),
                TaxAmount = g.Sum(s => s.TaxAmount),
                NetAmount = g.Sum(s => s.NetAmount),
                InvoiceCount = g.Count()
            }).OrderBy(r => r.Label).ToList();
        }

        if (groupBy == "Item")
        {
            return sales.SelectMany(s => s.Details)
                .GroupBy(d => d.ItemName)
                .Select(g => new SalesSummaryRowDto
                {
                    Label = g.Key,
                    Quantity = g.Sum(d => d.Quantity),
                    GrossAmount = g.Sum(d => d.Quantity * d.Price),
                    DiscountAmount = g.Sum(d => d.Discount),
                    TaxAmount = g.Sum(d => d.Tax),
                    NetAmount = g.Sum(d => d.Amount),
                    InvoiceCount = g.Select(d => d.SalesId).Distinct().Count()
                }).OrderByDescending(r => r.NetAmount).ToList();
        }

        if (groupBy == "Student")
        {
            var studentSales = sales.Where(s => s.StudentId.HasValue).ToList();
            var studentIds = studentSales.Select(s => s.StudentId!.Value).Distinct().ToList();
            var names = await _db.Students.AsNoTracking().Where(s => studentIds.Contains(s.StudentId))
                .ToDictionaryAsync(s => s.StudentId, s => $"{s.FirstName} {s.LastName}".Trim());
            return studentSales.GroupBy(s => s.StudentId!.Value).Select(g => new SalesSummaryRowDto
            {
                Label = names.TryGetValue(g.Key, out var n) ? n : $"Student #{g.Key}",
                Quantity = g.Sum(s => s.Details.Sum(d => d.Quantity)),
                GrossAmount = g.Sum(s => s.GrossAmount),
                DiscountAmount = g.Sum(s => s.DiscountAmount),
                TaxAmount = g.Sum(s => s.TaxAmount),
                NetAmount = g.Sum(s => s.NetAmount),
                InvoiceCount = g.Count()
            }).OrderByDescending(r => r.NetAmount).ToList();
        }

        // groupBy == "User" (cashier)
        var cashierIds = sales.Select(s => s.CashierId).Distinct().ToList();
        var cashierNames = await _db.Users.AsNoTracking().Where(u => cashierIds.Contains(u.UserId))
            .ToDictionaryAsync(u => u.UserId, u => u.FullName);
        return sales.GroupBy(s => s.CashierId).Select(g => new SalesSummaryRowDto
        {
            Label = cashierNames.TryGetValue(g.Key, out var n) ? n : $"User #{g.Key}",
            Quantity = g.Sum(s => s.Details.Sum(d => d.Quantity)),
            GrossAmount = g.Sum(s => s.GrossAmount),
            DiscountAmount = g.Sum(s => s.DiscountAmount),
            TaxAmount = g.Sum(s => s.TaxAmount),
            NetAmount = g.Sum(s => s.NetAmount),
            InvoiceCount = g.Count()
        }).OrderByDescending(r => r.NetAmount).ToList();
    }

    // ── Financial Reports ────────────────────────────────────────────────────
    public async Task<ProfitRowDto> GetGrossProfitAsync(DateTime? from, DateTime? to)
    {
        var lines = await GetCompletedSaleLinesAsync(from, to);
        var saleAmount = lines.Sum(d => d.Amount);
        var costAmount = lines.Sum(d => d.CostAmount);
        return new ProfitRowDto
        {
            Label = "Overall",
            QuantitySold = lines.Sum(d => d.Quantity),
            SaleAmount = saleAmount,
            CostAmount = costAmount,
            Profit = saleAmount - costAmount,
            MarginPercent = saleAmount > 0 ? Math.Round((saleAmount - costAmount) / saleAmount * 100, 2) : 0
        };
    }

    public async Task<List<ProfitRowDto>> GetProfitByItemAsync(DateTime? from, DateTime? to)
    {
        var lines = await GetCompletedSaleLinesAsync(from, to);
        return lines.GroupBy(d => d.ItemName).Select(g =>
        {
            var sale = g.Sum(d => d.Amount);
            var cost = g.Sum(d => d.CostAmount);
            return new ProfitRowDto
            {
                Label = g.Key,
                QuantitySold = g.Sum(d => d.Quantity),
                SaleAmount = sale,
                CostAmount = cost,
                Profit = sale - cost,
                MarginPercent = sale > 0 ? Math.Round((sale - cost) / sale * 100, 2) : 0
            };
        }).OrderByDescending(r => r.Profit).ToList();
    }

    public async Task<List<ProfitRowDto>> GetProfitByCategoryAsync(DateTime? from, DateTime? to)
    {
        var lines = await GetCompletedSaleLinesWithCategoryAsync(from, to);
        return lines.GroupBy(x => x.CategoryName).Select(g =>
        {
            var sale = g.Sum(x => x.Amount);
            var cost = g.Sum(x => x.CostAmount);
            return new ProfitRowDto
            {
                Label = g.Key,
                QuantitySold = g.Sum(x => x.Quantity),
                SaleAmount = sale,
                CostAmount = cost,
                Profit = sale - cost,
                MarginPercent = sale > 0 ? Math.Round((sale - cost) / sale * 100, 2) : 0
            };
        }).OrderByDescending(r => r.Profit).ToList();
    }

    // ── Package Reports ──────────────────────────────────────────────────────
    public async Task<List<PackageSalesRowDto>> GetPackageSalesAsync(DateTime? from, DateTime? to)
    {
        var q = _db.SalesDetails.AsNoTracking()
            .Include(d => d.Sales).Include(d => d.Package)
            .Where(d => d.PackageId != null &&
                (d.Sales.Status == "Completed" || d.Sales.Status == "PartiallyReturned" || d.Sales.Status == "Returned"))
            .AsQueryable();
        if (from.HasValue) q = q.Where(d => d.Sales.SalesDate >= from.Value);
        if (to.HasValue) q = q.Where(d => d.Sales.SalesDate <= to.Value);

        var lines = await q.ToListAsync();
        return lines.GroupBy(d => d.PackageId!.Value).Select(g => new PackageSalesRowDto
        {
            PackageId = g.Key,
            PackageName = g.First().Package?.PackageName ?? string.Empty,
            QuantitySold = g.Sum(d => d.Quantity),
            NetAmount = g.Sum(d => d.Amount),
            InvoiceCount = g.Select(d => d.SalesId).Distinct().Count()
        }).OrderByDescending(r => r.NetAmount).ToList();
    }

    // ── Shared helpers ───────────────────────────────────────────────────────
    private async Task<List<Models.SalesDetail>> GetCompletedSaleLinesAsync(DateTime? from, DateTime? to)
    {
        var q = _db.SalesDetails.AsNoTracking().Include(d => d.Sales)
            .Where(d => d.Sales.Status == "Completed" || d.Sales.Status == "PartiallyReturned" || d.Sales.Status == "Returned")
            .AsQueryable();
        if (from.HasValue) q = q.Where(d => d.Sales.SalesDate >= from.Value);
        if (to.HasValue) q = q.Where(d => d.Sales.SalesDate <= to.Value);
        return await q.ToListAsync();
    }

    private async Task<List<(string CategoryName, decimal Quantity, decimal Amount, decimal CostAmount)>> GetCompletedSaleLinesWithCategoryAsync(DateTime? from, DateTime? to)
    {
        var lines = await GetCompletedSaleLinesAsync(from, to);
        var itemIds = lines.Where(l => l.ItemId.HasValue).Select(l => l.ItemId!.Value).Distinct().ToList();
        var categories = await _db.ItemMasters.AsNoTracking().Include(i => i.Category)
            .Where(i => itemIds.Contains(i.ItemId))
            .ToDictionaryAsync(i => i.ItemId, i => i.Category.CategoryName);

        return lines.Select(l => (
            CategoryName: l.ItemId.HasValue && categories.TryGetValue(l.ItemId.Value, out var c) ? c : "Packages / Other",
            Quantity: l.Quantity,
            Amount: l.Amount,
            CostAmount: l.CostAmount
        )).ToList();
    }
}
