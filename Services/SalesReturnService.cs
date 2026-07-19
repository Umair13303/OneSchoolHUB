using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>Return of sold items (SRS §12) — increases stock, reverses movement, updates sales history.</summary>
public interface ISalesReturnService
{
    Task<List<SalesReturnDto>> GetAllAsync();
    Task<SalesReturnDto?> GetByIdAsync(int id);
    Task<SalesReturnDto> CreateAsync(CreateSalesReturnDto dto, int userId);
}

public class SalesReturnService : ISalesReturnService
{
    private readonly AppDbContext _db;
    private readonly IStockService _stock;
    public SalesReturnService(AppDbContext db, IStockService stock) { _db = db; _stock = stock; }

    public async Task<List<SalesReturnDto>> GetAllAsync()
    {
        var list = await _db.SalesReturnMasters.AsNoTracking()
            .Include(r => r.Sales).Include(r => r.Details).ThenInclude(d => d.Item)
            .Include(r => r.Details).ThenInclude(d => d.Package)
            .OrderByDescending(r => r.ReturnDate).ThenByDescending(r => r.SalesReturnId)
            .ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<SalesReturnDto?> GetByIdAsync(int id)
    {
        var entity = await _db.SalesReturnMasters.AsNoTracking()
            .Include(r => r.Sales).Include(r => r.Details).ThenInclude(d => d.Item)
            .Include(r => r.Details).ThenInclude(d => d.Package)
            .FirstOrDefaultAsync(r => r.SalesReturnId == id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<SalesReturnDto> CreateAsync(CreateSalesReturnDto dto, int userId)
    {
        if (dto.Lines.Count == 0) throw new ArgumentException("A return must have at least one line.");

        var sale = await _db.SalesMasters.Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.SalesId == dto.SalesId)
            ?? throw new ArgumentException("Sale invoice not found.");
        if (sale.Status is not ("Completed" or "PartiallyReturned"))
            throw new InvalidOperationException("Only a completed sale can be returned.");

        // Sum of already-returned quantity per ORIGINAL sales-detail line (not per item —
        // two lines for the same item/package on one invoice must be capped independently),
        // to prevent over-returning.
        var alreadyReturned = await _db.SalesReturnDetails
            .Where(d => d.SalesReturn.SalesId == sale.SalesId)
            .GroupBy(d => d.SalesDetailId)
            .Select(g => new { SalesDetailId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToListAsync();

        var returnNo = await _stock.NextVoucherNoAsync("SRTN", () => _db.SalesReturnMasters.IgnoreQueryFilters().CountAsync());
        var master = new SalesReturnMaster
        {
            ReturnNo = returnNo,
            ReturnDate = DateTime.UtcNow,
            SalesId = sale.SalesId,
            Remarks = dto.Remarks,
            Status = "Posted",
            CreatedBy = userId
        };

        decimal net = 0;

        foreach (var line in dto.Lines)
        {
            var original = sale.Details.FirstOrDefault(d => d.SalesDetailId == line.SalesDetailId)
                ?? throw new ArgumentException($"Sale line {line.SalesDetailId} does not belong to this invoice.");
            if (line.Quantity <= 0) throw new ArgumentException("Return quantity must be greater than zero.");

            var returnedSoFar = alreadyReturned.FirstOrDefault(a => a.SalesDetailId == original.SalesDetailId)?.Qty ?? 0;
            if (returnedSoFar + line.Quantity > original.Quantity)
                throw new InvalidOperationException($"Cannot return more than was sold for '{original.ItemName}'.");

            var lineAmount = line.Quantity * original.Price;
            master.Details.Add(new SalesReturnDetail
            {
                SalesDetailId = original.SalesDetailId,
                ItemId = original.ItemId,
                PackageId = original.PackageId,
                Quantity = line.Quantity,
                Price = original.Price,
                Amount = lineAmount
            });
            net += lineAmount;

            if (original.ItemId.HasValue)
            {
                var item = await _db.ItemMasters.FirstAsync(i => i.ItemId == original.ItemId.Value);
                await _stock.PostMovementAsync(item.ItemId, "SalesReturn", returnNo, line.Quantity, 0, item.AverageCost, userId);
            }
            else if (original.PackageId.HasValue)
            {
                var pkgDetails = await _db.PackageDetails.Include(d => d.Item)
                    .Where(d => d.PackageId == original.PackageId.Value).ToListAsync();
                foreach (var pd in pkgDetails)
                    await _stock.PostMovementAsync(pd.ItemId, "SalesReturn", returnNo, line.Quantity * pd.Quantity, 0, pd.Item.AverageCost, userId);
            }
        }

        master.NetAmount = net;
        _db.SalesReturnMasters.Add(master);

        // Update parent sale status.
        var totalOriginal = sale.Details.Sum(d => d.Quantity);
        var totalReturnedNow = alreadyReturned.Sum(a => a.Qty) + dto.Lines.Sum(l => l.Quantity);
        sale.Status = totalReturnedNow >= totalOriginal ? "Returned" : "PartiallyReturned";
        sale.UpdatedBy = userId;
        sale.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (await GetByIdAsync(master.SalesReturnId))!;
    }

    private static SalesReturnDto MapToDto(SalesReturnMaster r) => new()
    {
        SalesReturnId = r.SalesReturnId,
        ReturnNo = r.ReturnNo,
        ReturnDate = r.ReturnDate,
        SalesId = r.SalesId,
        InvoiceNo = r.Sales?.InvoiceNo ?? string.Empty,
        Remarks = r.Remarks,
        NetAmount = r.NetAmount,
        Status = r.Status,
        Details = r.Details.Select(d => new SalesReturnDetailDto
        {
            SalesDetailId = d.SalesDetailId,
            ItemId = d.ItemId,
            PackageId = d.PackageId,
            ItemName = d.Item?.ItemName ?? d.Package?.PackageName ?? string.Empty,
            Quantity = d.Quantity,
            Price = d.Price,
            Amount = d.Amount
        }).ToList()
    };
}
