using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>
/// Purchase invoices. Posting a purchase (SRS §3 "System Behavior"):
///   • increases stock (via IStockService)
///   • updates Last Purchase Price
///   • recalculates Average Cost per the configured costing method
///   • writes a Stock Ledger entry per line
/// </summary>
public interface IPurchaseService
{
    Task<List<PurchaseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? supplierId);
    Task<PurchaseDto?> GetByIdAsync(int id);
    Task<PurchaseDto> CreateAsync(CreatePurchaseDto dto, int userId);
    Task<bool> CancelAsync(int id, int userId);
}

public class PurchaseService : IPurchaseService
{
    private readonly AppDbContext _db;
    private readonly IStockService _stock;
    public PurchaseService(AppDbContext db, IStockService stock) { _db = db; _stock = stock; }

    public async Task<List<PurchaseDto>> GetAllAsync(DateOnly? from, DateOnly? to, int? supplierId)
    {
        var q = _db.PurchaseMasters.AsNoTracking()
            .Include(p => p.Supplier).Include(p => p.Details).ThenInclude(d => d.Item)
            .AsQueryable();
        if (from.HasValue) q = q.Where(p => p.PurchaseDate >= from.Value);
        if (to.HasValue) q = q.Where(p => p.PurchaseDate <= to.Value);
        if (supplierId.HasValue) q = q.Where(p => p.SupplierId == supplierId.Value);

        var list = await q.OrderByDescending(p => p.PurchaseDate).ThenByDescending(p => p.PurchaseId).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<PurchaseDto?> GetByIdAsync(int id)
    {
        var entity = await _db.PurchaseMasters.AsNoTracking()
            .Include(p => p.Supplier).Include(p => p.Details).ThenInclude(d => d.Item)
            .FirstOrDefaultAsync(p => p.PurchaseId == id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<PurchaseDto> CreateAsync(CreatePurchaseDto dto, int userId)
    {
        if (dto.Details.Count == 0) throw new ArgumentException("A purchase must have at least one item line.");
        if (!await _db.Suppliers.AnyAsync(s => s.SupplierId == dto.SupplierId))
            throw new ArgumentException("Selected supplier does not exist.");

        var purchaseNo = await _stock.NextVoucherNoAsync("PUR", () => _db.PurchaseMasters.IgnoreQueryFilters().CountAsync());

        var master = new PurchaseMaster
        {
            PurchaseNo = purchaseNo,
            PurchaseDate = dto.PurchaseDate,
            SupplierId = dto.SupplierId,
            InvoiceNumber = dto.InvoiceNumber,
            Remarks = dto.Remarks,
            Status = "Posted",
            CreatedBy = userId
        };

        decimal gross = 0, discount = 0, tax = 0, net = 0;
        var transactionDate = dto.PurchaseDate.ToDateTime(TimeOnly.MinValue);

        foreach (var line in dto.Details)
        {
            if (line.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
            var item = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == line.ItemId)
                ?? throw new ArgumentException($"Item {line.ItemId} not found.");

            var lineGross = line.Quantity * line.PurchasePrice;
            var lineNet = lineGross - line.Discount + line.Tax;

            master.Details.Add(new PurchaseDetail
            {
                ItemId = line.ItemId,
                Quantity = line.Quantity,
                PurchasePrice = line.PurchasePrice,
                Discount = line.Discount,
                Tax = line.Tax,
                NetAmount = lineNet
            });

            gross += lineGross; discount += line.Discount; tax += line.Tax; net += lineNet;

            // Update costing BEFORE posting the stock movement so ledger Cost reflects the new average.
            await _stock.ApplyPurchaseCostingAsync(item, line.Quantity, line.PurchasePrice);
            await _stock.PostMovementAsync(item.ItemId, "Purchase", purchaseNo, line.Quantity, 0, item.AverageCost, userId, transactionDate);
        }

        master.GrossAmount = gross;
        master.DiscountAmount = discount;
        master.TaxAmount = tax;
        master.NetAmount = net;

        _db.PurchaseMasters.Add(master);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(master.PurchaseId))!;
    }

    public async Task<bool> CancelAsync(int id, int userId)
    {
        var entity = await _db.PurchaseMasters.Include(p => p.Details).FirstOrDefaultAsync(p => p.PurchaseId == id);
        if (entity is null) return false;
        if (entity.Status == "Cancelled") return true;

        // Reverse stock for each line (does not roll back AverageCost/LastPurchasePrice —
        // matching most real-world POS/ERP behavior where cost history is not rewritten).
        var transactionDate = DateTime.UtcNow;
        foreach (var d in entity.Details)
        {
            var item = await _db.ItemMasters.FirstAsync(i => i.ItemId == d.ItemId);
            await _stock.PostMovementAsync(d.ItemId, "PurchaseCancelled", entity.PurchaseNo, 0, d.Quantity, item.AverageCost, userId, transactionDate);
        }

        entity.Status = "Cancelled";
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private static PurchaseDto MapToDto(PurchaseMaster p) => new()
    {
        PurchaseId = p.PurchaseId,
        PurchaseNo = p.PurchaseNo,
        PurchaseDate = p.PurchaseDate,
        SupplierId = p.SupplierId,
        SupplierName = p.Supplier?.SupplierName ?? string.Empty,
        InvoiceNumber = p.InvoiceNumber,
        Remarks = p.Remarks,
        GrossAmount = p.GrossAmount,
        DiscountAmount = p.DiscountAmount,
        TaxAmount = p.TaxAmount,
        NetAmount = p.NetAmount,
        Status = p.Status,
        Details = p.Details.Select(d => new PurchaseDetailDto
        {
            PurchaseDetailId = d.PurchaseDetailId,
            ItemId = d.ItemId,
            ItemName = d.Item?.ItemName ?? string.Empty,
            Quantity = d.Quantity,
            PurchasePrice = d.PurchasePrice,
            Discount = d.Discount,
            Tax = d.Tax,
            NetAmount = d.NetAmount
        }).ToList()
    };
}
