using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>
/// Point of Sale — the SRS §7 checkout engine. Supports individual items and
/// package (bundle) sales, holding/resuming invoices, multiple payment methods,
/// and student sales. Stock is only touched when an invoice is Completed —
/// a Held invoice has zero stock/financial impact until resumed.
/// </summary>
public interface IPosService
{
    Task<List<PosLookupDto>> SearchAsync(string term);
    Task<SalesDto> CreateSaleAsync(CreateSaleDto dto, int cashierId);
    Task<SalesDto> CompleteSaleAsync(int id, CompleteSaleDto dto, int cashierId);
    Task<bool> VoidHeldInvoiceAsync(int id, int userId);
    Task<List<SalesDto>> GetHeldInvoicesAsync();
    Task<SalesDto?> GetByIdAsync(int id);
    Task<List<SalesDto>> GetAllAsync(DateTime? from, DateTime? to, int? cashierId, int? studentId, string? status);
}

public class PosService : IPosService
{
    private readonly AppDbContext _db;
    private readonly IStockService _stock;
    private readonly IItemService _items;

    public PosService(AppDbContext db, IStockService stock, IItemService items)
    {
        _db = db; _stock = stock; _items = items;
    }

    public Task<List<PosLookupDto>> SearchAsync(string term) => _items.SearchForPosAsync(term);

    public async Task<SalesDto> CreateSaleAsync(CreateSaleDto dto, int cashierId)
    {
        if (dto.Lines.Count == 0) throw new ArgumentException("The cart is empty.");

        var invoiceNo = await _stock.NextVoucherNoAsync("INV", () => _db.SalesMasters.IgnoreQueryFilters().CountAsync());
        var master = new SalesMaster
        {
            InvoiceNo = invoiceNo,
            SalesDate = DateTime.UtcNow,
            StudentId = dto.StudentId,
            CustomerName = dto.CustomerName,
            Remarks = dto.Remarks,
            HoldReference = dto.HoldReference,
            Status = dto.Hold ? "Held" : "Completed",
            CashierId = cashierId,
            CreatedBy = cashierId
        };

        await BuildLinesAsync(master, dto.Lines);

        if (!dto.Hold)
        {
            await PostStockForLinesAsync(master, cashierId);
            await ApplyPaymentsAsync(master, dto.Payments);
        }

        _db.SalesMasters.Add(master);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(master.SalesId))!;
    }

    public async Task<SalesDto> CompleteSaleAsync(int id, CompleteSaleDto dto, int cashierId)
    {
        var master = await _db.SalesMasters.Include(s => s.Details).Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.SalesId == id)
            ?? throw new ArgumentException("Invoice not found.");
        if (master.Status != "Held")
            throw new InvalidOperationException("Only a held invoice can be resumed/completed.");

        if (dto.Lines is { Count: > 0 })
        {
            _db.SalesDetails.RemoveRange(master.Details);
            master.Details.Clear();
            await BuildLinesAsync(master, dto.Lines);
        }

        master.CashierId = cashierId;
        master.SalesDate = DateTime.UtcNow;
        master.Status = "Completed";

        await PostStockForLinesAsync(master, cashierId);
        await ApplyPaymentsAsync(master, dto.Payments);

        await _db.SaveChangesAsync();
        return (await GetByIdAsync(master.SalesId))!;
    }

    public async Task<bool> VoidHeldInvoiceAsync(int id, int userId)
    {
        var master = await _db.SalesMasters.FirstOrDefaultAsync(s => s.SalesId == id);
        if (master is null) return false;
        if (master.Status != "Held") throw new InvalidOperationException("Only a held invoice can be voided directly — use Sales Return for completed invoices.");
        master.Status = "Cancelled";
        master.UpdatedBy = userId;
        master.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<SalesDto>> GetHeldInvoicesAsync()
    {
        var list = await _db.SalesMasters.AsNoTracking()
            .Where(s => s.Status == "Held")
            .Include(s => s.Details).Include(s => s.Payments)
            .OrderByDescending(s => s.SalesDate).ToListAsync();
        return await MapManyToDtoAsync(list);
    }

    public async Task<SalesDto?> GetByIdAsync(int id)
    {
        var entity = await _db.SalesMasters.AsNoTracking()
            .Include(s => s.Details).Include(s => s.Payments)
            .FirstOrDefaultAsync(s => s.SalesId == id);
        if (entity is null) return null;
        return (await MapManyToDtoAsync(new List<SalesMaster> { entity })).First();
    }

    public async Task<List<SalesDto>> GetAllAsync(DateTime? from, DateTime? to, int? cashierId, int? studentId, string? status)
    {
        var q = _db.SalesMasters.AsNoTracking().Include(s => s.Details).Include(s => s.Payments).AsQueryable();
        if (from.HasValue) q = q.Where(s => s.SalesDate >= from.Value);
        if (to.HasValue) q = q.Where(s => s.SalesDate <= to.Value);
        if (cashierId.HasValue) q = q.Where(s => s.CashierId == cashierId.Value);
        if (studentId.HasValue) q = q.Where(s => s.StudentId == studentId.Value);
        if (!string.IsNullOrWhiteSpace(status)) q = q.Where(s => s.Status == status);

        var list = await q.OrderByDescending(s => s.SalesDate).Take(1000).ToListAsync();
        return await MapManyToDtoAsync(list);
    }

    // ── Internal helpers ─────────────────────────────────────────────────────
    private async Task BuildLinesAsync(SalesMaster master, List<CreateSaleLineDto> lines)
    {
        decimal gross = 0, discount = 0, tax = 0, net = 0;

        foreach (var line in lines)
        {
            if (line.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
            string name;

            if (string.Equals(line.Kind, "Package", StringComparison.OrdinalIgnoreCase))
            {
                var pkg = await _db.PackageMasters.FirstOrDefaultAsync(p => p.PackageId == line.Id && p.IsActive)
                    ?? throw new ArgumentException($"Package {line.Id} not found or inactive.");
                name = pkg.PackageName;
                var pkgDetails = await _db.PackageDetails.Include(d => d.Item)
                    .Where(d => d.PackageId == pkg.PackageId).ToListAsync();
                var unitCost = pkgDetails.Sum(d => d.Quantity * d.Item.AverageCost);

                master.Details.Add(new SalesDetail
                {
                    PackageId = pkg.PackageId,
                    ItemName = name,
                    Quantity = line.Quantity,
                    Price = line.Price,
                    Discount = line.Discount,
                    Tax = line.Tax,
                    Amount = line.Quantity * line.Price - line.Discount + line.Tax,
                    CostAmount = line.Quantity * unitCost
                });
            }
            else
            {
                var item = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == line.Id && i.IsActive)
                    ?? throw new ArgumentException($"Item {line.Id} not found or inactive.");
                name = item.ItemName;
                master.Details.Add(new SalesDetail
                {
                    ItemId = item.ItemId,
                    ItemName = name,
                    Quantity = line.Quantity,
                    Price = line.Price,
                    Discount = line.Discount,
                    Tax = line.Tax,
                    Amount = line.Quantity * line.Price - line.Discount + line.Tax,
                    CostAmount = line.Quantity * item.AverageCost
                });
            }

            gross += line.Quantity * line.Price;
            discount += line.Discount;
            tax += line.Tax;
            net += line.Quantity * line.Price - line.Discount + line.Tax;
        }

        master.GrossAmount = gross;
        master.DiscountAmount = discount;
        master.TaxAmount = tax;
        master.NetAmount = net;
    }

    private async Task PostStockForLinesAsync(SalesMaster master, int userId)
    {
        var transactionDate = master.SalesDate;
        foreach (var line in master.Details)
        {
            if (line.ItemId.HasValue)
            {
                var item = await _db.ItemMasters.FirstAsync(i => i.ItemId == line.ItemId.Value);
                await _stock.PostMovementAsync(item.ItemId, "Sale", master.InvoiceNo, 0, line.Quantity, item.AverageCost, userId, transactionDate);
            }
            else if (line.PackageId.HasValue)
            {
                var pkgDetails = await _db.PackageDetails.Include(d => d.Item)
                    .Where(d => d.PackageId == line.PackageId.Value).ToListAsync();
                foreach (var pd in pkgDetails)
                {
                    var qty = line.Quantity * pd.Quantity;
                    await _stock.PostMovementAsync(pd.ItemId, "PackageSale", master.InvoiceNo, 0, qty, pd.Item.AverageCost, userId, transactionDate);
                }
            }
        }
    }

    private Task ApplyPaymentsAsync(SalesMaster master, List<CreatePaymentLineDto> payments)
    {
        if (payments.Count == 0)
            throw new ArgumentException("At least one payment line is required to complete a sale.");

        foreach (var p in payments)
        {
            if (p.Amount <= 0) throw new ArgumentException("Payment amount must be greater than zero.");
            master.Payments.Add(new SalesPayment { PaymentMethod = p.PaymentMethod, Amount = p.Amount, ReferenceNo = p.ReferenceNo });
        }

        var paid = payments.Sum(p => p.Amount);
        if (paid < master.NetAmount)
            throw new InvalidOperationException($"Payments ({paid:0.00}) are less than the invoice total ({master.NetAmount:0.00}).");

        master.PaidAmount = paid;
        master.ChangeAmount = paid - master.NetAmount;
        return Task.CompletedTask;
    }

    private async Task<List<SalesDto>> MapManyToDtoAsync(List<SalesMaster> list)
    {
        var studentIds = list.Where(s => s.StudentId.HasValue).Select(s => s.StudentId!.Value).Distinct().ToList();
        var studentNames = studentIds.Count == 0
            ? new Dictionary<int, string>()
            : await _db.Students.AsNoTracking().Where(s => studentIds.Contains(s.StudentId))
                .ToDictionaryAsync(s => s.StudentId, s => $"{s.FirstName} {s.LastName}".Trim());

        var cashierIds = list.Select(s => s.CashierId).Distinct().ToList();
        var cashierNames = await _db.Users.AsNoTracking().Where(u => cashierIds.Contains(u.UserId))
            .ToDictionaryAsync(u => u.UserId, u => u.FullName);

        return list.Select(s => new SalesDto
        {
            SalesId = s.SalesId,
            InvoiceNo = s.InvoiceNo,
            SalesDate = s.SalesDate,
            StudentId = s.StudentId,
            StudentName = s.StudentId.HasValue && studentNames.TryGetValue(s.StudentId.Value, out var sn) ? sn : null,
            CustomerName = s.CustomerName,
            GrossAmount = s.GrossAmount,
            DiscountAmount = s.DiscountAmount,
            TaxAmount = s.TaxAmount,
            NetAmount = s.NetAmount,
            PaidAmount = s.PaidAmount,
            ChangeAmount = s.ChangeAmount,
            Status = s.Status,
            CashierId = s.CashierId,
            CashierName = cashierNames.TryGetValue(s.CashierId, out var cn) ? cn : string.Empty,
            Remarks = s.Remarks,
            HoldReference = s.HoldReference,
            Details = s.Details.Select(d => new SalesDetailDto
            {
                SalesDetailId = d.SalesDetailId,
                ItemId = d.ItemId,
                PackageId = d.PackageId,
                ItemName = d.ItemName,
                Quantity = d.Quantity,
                Price = d.Price,
                Discount = d.Discount,
                Tax = d.Tax,
                Amount = d.Amount
            }).ToList(),
            Payments = s.Payments.Select(p => new SalesPaymentDto
            {
                SalesPaymentId = p.SalesPaymentId,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                ReferenceNo = p.ReferenceNo
            }).ToList()
        }).ToList();
    }
}
