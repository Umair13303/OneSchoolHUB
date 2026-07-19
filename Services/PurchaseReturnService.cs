using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>Returning purchased items to a supplier — reduces stock and writes the ledger (SRS §11).</summary>
public interface IPurchaseReturnService
{
    Task<List<PurchaseReturnDto>> GetAllAsync();
    Task<PurchaseReturnDto?> GetByIdAsync(int id);
    Task<PurchaseReturnDto> CreateAsync(CreatePurchaseReturnDto dto, int userId);
}

public class PurchaseReturnService : IPurchaseReturnService
{
    private readonly AppDbContext _db;
    private readonly IStockService _stock;
    public PurchaseReturnService(AppDbContext db, IStockService stock) { _db = db; _stock = stock; }

    public async Task<List<PurchaseReturnDto>> GetAllAsync()
    {
        var list = await _db.PurchaseReturnMasters.AsNoTracking()
            .Include(r => r.Supplier).Include(r => r.Details).ThenInclude(d => d.Item)
            .OrderByDescending(r => r.ReturnDate).ThenByDescending(r => r.PurchaseReturnId)
            .ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<PurchaseReturnDto?> GetByIdAsync(int id)
    {
        var entity = await _db.PurchaseReturnMasters.AsNoTracking()
            .Include(r => r.Supplier).Include(r => r.Details).ThenInclude(d => d.Item)
            .FirstOrDefaultAsync(r => r.PurchaseReturnId == id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<PurchaseReturnDto> CreateAsync(CreatePurchaseReturnDto dto, int userId)
    {
        if (dto.Details.Count == 0) throw new ArgumentException("A return must have at least one item line.");
        if (!await _db.Suppliers.AnyAsync(s => s.SupplierId == dto.SupplierId))
            throw new ArgumentException("Selected supplier does not exist.");

        var returnNo = await _stock.NextVoucherNoAsync("PRTN", () => _db.PurchaseReturnMasters.IgnoreQueryFilters().CountAsync());
        var master = new PurchaseReturnMaster
        {
            ReturnNo = returnNo,
            ReturnDate = dto.ReturnDate,
            PurchaseId = dto.PurchaseId,
            SupplierId = dto.SupplierId,
            Remarks = dto.Remarks,
            Status = "Posted",
            CreatedBy = userId
        };

        decimal net = 0;
        var transactionDate = dto.ReturnDate.ToDateTime(TimeOnly.MinValue);

        foreach (var line in dto.Details)
        {
            if (line.Quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
            var item = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == line.ItemId)
                ?? throw new ArgumentException($"Item {line.ItemId} not found.");

            var lineNet = line.Quantity * line.Price;
            master.Details.Add(new PurchaseReturnDetail
            {
                ItemId = line.ItemId,
                Quantity = line.Quantity,
                Price = line.Price,
                NetAmount = lineNet
            });
            net += lineNet;

            await _stock.PostMovementAsync(item.ItemId, "PurchaseReturn", returnNo, 0, line.Quantity, item.AverageCost, userId, transactionDate);
        }

        master.NetAmount = net;
        _db.PurchaseReturnMasters.Add(master);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(master.PurchaseReturnId))!;
    }

    private static PurchaseReturnDto MapToDto(PurchaseReturnMaster r) => new()
    {
        PurchaseReturnId = r.PurchaseReturnId,
        ReturnNo = r.ReturnNo,
        ReturnDate = r.ReturnDate,
        PurchaseId = r.PurchaseId,
        SupplierId = r.SupplierId,
        SupplierName = r.Supplier?.SupplierName ?? string.Empty,
        Remarks = r.Remarks,
        NetAmount = r.NetAmount,
        Status = r.Status,
        Details = r.Details.Select(d => new PurchaseReturnDetailDto
        {
            ItemId = d.ItemId,
            ItemName = d.Item?.ItemName ?? string.Empty,
            Quantity = d.Quantity,
            Price = d.Price,
            NetAmount = d.NetAmount
        }).ToList()
    };
}
