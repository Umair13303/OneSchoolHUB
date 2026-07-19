using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IItemService
{
    Task<List<ItemDto>> GetAllAsync(int? categoryId, string? search, bool activeOnly = false);
    Task<ItemDto?> GetByIdAsync(int id);
    Task<ItemDto> CreateAsync(CreateItemDto dto, int userId);
    Task<bool> UpdateAsync(int id, UpdateItemDto dto, int userId, bool canEditSalePrice);
    Task<bool> DeleteAsync(int id, int userId);
    Task<bool> ToggleStatusAsync(int id, int userId);
    Task<List<PosLookupDto>> SearchForPosAsync(string term);
    Task<ItemDto?> FindByBarcodeAsync(string barcode);
}

public class ItemService : IItemService
{
    private readonly AppDbContext _db;
    private readonly IStockService _stock;
    public ItemService(AppDbContext db, IStockService stock) { _db = db; _stock = stock; }

    public async Task<List<ItemDto>> GetAllAsync(int? categoryId, string? search, bool activeOnly = false)
    {
        var q = _db.ItemMasters.AsNoTracking().Include(i => i.Category).Include(i => i.Unit).AsQueryable();
        if (categoryId.HasValue) q = q.Where(i => i.ItemCategoryId == categoryId.Value);
        if (activeOnly) q = q.Where(i => i.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(i => i.ItemName.Contains(search) || i.ItemCode.Contains(search) || (i.Barcode != null && i.Barcode.Contains(search)));

        var items = await q.OrderBy(i => i.ItemName).ToListAsync();
        var stockMap = await _db.CurrentStocks.AsNoTracking().ToDictionaryAsync(c => c.ItemId, c => c.QuantityOnHand);
        return items.Select(i => MapToDto(i, stockMap)).ToList();
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _db.ItemMasters.AsNoTracking().Include(i => i.Category).Include(i => i.Unit)
            .FirstOrDefaultAsync(i => i.ItemId == id);
        if (item is null) return null;
        var qty = await _stock.GetQuantityOnHandAsync(id);
        return MapToDto(item, new Dictionary<int, decimal> { [id] = qty });
    }

    public async Task<ItemDto> CreateAsync(CreateItemDto dto, int userId)
    {
        if (!await _db.ItemCategories.AnyAsync(c => c.ItemCategoryId == dto.ItemCategoryId))
            throw new ArgumentException("Selected category does not exist.");
        if (!await _db.UnitMasters.AnyAsync(u => u.UnitId == dto.UnitId))
            throw new ArgumentException("Selected unit does not exist.");

        var code = dto.ItemCode;
        if (string.IsNullOrWhiteSpace(code))
        {
            var count = await _db.ItemMasters.IgnoreQueryFilters().CountAsync();
            code = $"ITM-{(count + 1):D6}";
        }
        else if (await _db.ItemMasters.AnyAsync(i => i.ItemCode == code))
        {
            throw new InvalidOperationException($"Item code '{code}' already exists.");
        }

        if (!string.IsNullOrWhiteSpace(dto.Barcode) && await _db.ItemMasters.AnyAsync(i => i.Barcode == dto.Barcode))
            throw new InvalidOperationException($"Barcode '{dto.Barcode}' is already assigned to another item.");

        var entity = new ItemMaster
        {
            ItemCode = code,
            Barcode = string.IsNullOrWhiteSpace(dto.Barcode) ? null : dto.Barcode,
            ItemName = dto.ItemName,
            ItemCategoryId = dto.ItemCategoryId,
            Brand = dto.Brand,
            UnitId = dto.UnitId,
            Description = dto.Description,
            MinimumStockLevel = dto.MinimumStockLevel,
            ReorderLevel = dto.ReorderLevel,
            SalePrice = dto.SalePrice,
            WholesalePrice = dto.WholesalePrice,
            MinimumSalePrice = dto.MinimumSalePrice,
            TaxPercentage = dto.TaxPercentage,
            LastPurchasePrice = dto.OpeningCost,
            AverageCost = dto.OpeningCost,
            IsActive = true,
            CreatedBy = userId
        };
        // The item insert and its opening-stock movement must succeed or fail together —
        // wrap both SaveChangesAsync calls (the second needs entity.ItemId from the first)
        // in one transaction so a failure never leaves an item with no stock/ledger row.
        await using var tx = await _db.Database.BeginTransactionAsync();
        try
        {
            _db.ItemMasters.Add(entity);
            await _db.SaveChangesAsync();

            if (dto.OpeningQuantity > 0)
            {
                await _stock.PostMovementAsync(entity.ItemId, "OpeningStock", $"OPEN-{entity.ItemId}", dto.OpeningQuantity, 0, dto.OpeningCost, userId);
                await _db.SaveChangesAsync();
            }

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }

        return (await GetByIdAsync(entity.ItemId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateItemDto dto, int userId, bool canEditSalePrice)
    {
        var entity = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == id);
        if (entity is null) return false;

        if (!string.IsNullOrWhiteSpace(dto.Barcode) &&
            await _db.ItemMasters.AnyAsync(i => i.Barcode == dto.Barcode && i.ItemId != id))
            throw new InvalidOperationException($"Barcode '{dto.Barcode}' is already assigned to another item.");
        if (await _db.ItemMasters.AnyAsync(i => i.ItemCode == dto.ItemCode && i.ItemId != id))
            throw new InvalidOperationException($"Item code '{dto.ItemCode}' already exists.");

        entity.ItemCode = dto.ItemCode;
        entity.Barcode = string.IsNullOrWhiteSpace(dto.Barcode) ? null : dto.Barcode;
        entity.ItemName = dto.ItemName;
        entity.ItemCategoryId = dto.ItemCategoryId;
        entity.Brand = dto.Brand;
        entity.UnitId = dto.UnitId;
        entity.Description = dto.Description;
        entity.MinimumStockLevel = dto.MinimumStockLevel;
        entity.ReorderLevel = dto.ReorderLevel;
        entity.TaxPercentage = dto.TaxPercentage;
        entity.IsActive = dto.IsActive;

        // SRS: "Sale price is editable by authorized users only."
        if (canEditSalePrice)
        {
            entity.SalePrice = dto.SalePrice;
            entity.WholesalePrice = dto.WholesalePrice;
            entity.MinimumSalePrice = dto.MinimumSalePrice;
        }

        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == id);
        if (entity is null) return false;
        if (await _db.PurchaseDetails.AnyAsync(d => d.ItemId == id) || await _db.SalesDetails.AnyAsync(d => d.ItemId == id))
            throw new InvalidOperationException("Cannot delete an item that has purchase or sales history — deactivate it instead.");

        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleStatusAsync(int id, int userId)
    {
        var entity = await _db.ItemMasters.FirstOrDefaultAsync(i => i.ItemId == id);
        if (entity is null) return false;
        entity.IsActive = !entity.IsActive;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>Fast combined item+package lookup for the POS search/barcode box.</summary>
    public async Task<List<PosLookupDto>> SearchForPosAsync(string term)
    {
        term ??= string.Empty;
        var items = await _db.ItemMasters.AsNoTracking()
            .Where(i => i.IsActive && (i.ItemName.Contains(term) || i.ItemCode.Contains(term) || (i.Barcode != null && i.Barcode == term)))
            .Take(25).ToListAsync();
        var stockMap = await _db.CurrentStocks.AsNoTracking()
            .Where(c => items.Select(i => i.ItemId).Contains(c.ItemId))
            .ToDictionaryAsync(c => c.ItemId, c => c.QuantityOnHand);

        var itemResults = items.Select(i => new PosLookupDto
        {
            Kind = "Item",
            Id = i.ItemId,
            Code = i.ItemCode,
            Barcode = i.Barcode,
            Name = i.ItemName,
            Price = i.SalePrice,
            TaxPercentage = i.TaxPercentage,
            QuantityOnHand = stockMap.TryGetValue(i.ItemId, out var q) ? q : 0m
        });

        var packages = await _db.PackageMasters.AsNoTracking()
            .Where(p => p.IsActive && (p.PackageName.Contains(term) || p.PackageCode.Contains(term)))
            .Take(10).ToListAsync();
        var packageResults = packages.Select(p => new PosLookupDto
        {
            Kind = "Package",
            Id = p.PackageId,
            Code = p.PackageCode,
            Name = p.PackageName,
            Price = p.PackagePrice,
            TaxPercentage = 0,
            QuantityOnHand = 0
        });

        return itemResults.Concat(packageResults).ToList();
    }

    public async Task<ItemDto?> FindByBarcodeAsync(string barcode)
    {
        var item = await _db.ItemMasters.AsNoTracking().Include(i => i.Category).Include(i => i.Unit)
            .FirstOrDefaultAsync(i => i.Barcode == barcode && i.IsActive);
        if (item is null) return null;
        var qty = await _stock.GetQuantityOnHandAsync(item.ItemId);
        return MapToDto(item, new Dictionary<int, decimal> { [item.ItemId] = qty });
    }

    private static ItemDto MapToDto(ItemMaster i, Dictionary<int, decimal> stockMap) => new()
    {
        ItemId = i.ItemId,
        ItemCode = i.ItemCode,
        Barcode = i.Barcode,
        ItemName = i.ItemName,
        ItemCategoryId = i.ItemCategoryId,
        CategoryName = i.Category?.CategoryName ?? string.Empty,
        Brand = i.Brand,
        UnitId = i.UnitId,
        UnitName = i.Unit?.UnitName ?? string.Empty,
        Description = i.Description,
        MinimumStockLevel = i.MinimumStockLevel,
        ReorderLevel = i.ReorderLevel,
        LastPurchasePrice = i.LastPurchasePrice,
        AverageCost = i.AverageCost,
        SalePrice = i.SalePrice,
        WholesalePrice = i.WholesalePrice,
        MinimumSalePrice = i.MinimumSalePrice,
        TaxPercentage = i.TaxPercentage,
        IsActive = i.IsActive,
        QuantityOnHand = stockMap.TryGetValue(i.ItemId, out var q) ? q : 0m
    };
}
