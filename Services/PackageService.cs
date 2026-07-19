using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IPackageService
{
    Task<List<PackageDto>> GetAllAsync(bool activeOnly = false);
    Task<PackageDto?> GetByIdAsync(int id);
    Task<PackageDto> CreateAsync(CreatePackageDto dto, int userId);
    Task<bool> UpdateAsync(int id, UpdatePackageDto dto, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}

public class PackageService : IPackageService
{
    private readonly AppDbContext _db;
    public PackageService(AppDbContext db) => _db = db;

    public async Task<List<PackageDto>> GetAllAsync(bool activeOnly = false)
    {
        var q = _db.PackageMasters.AsNoTracking()
            .Include(p => p.Details).ThenInclude(d => d.Item)
            .AsQueryable();
        if (activeOnly) q = q.Where(p => p.IsActive);
        var list = await q.OrderBy(p => p.PackageName).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<PackageDto?> GetByIdAsync(int id)
    {
        var entity = await _db.PackageMasters.AsNoTracking()
            .Include(p => p.Details).ThenInclude(d => d.Item)
            .FirstOrDefaultAsync(p => p.PackageId == id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<PackageDto> CreateAsync(CreatePackageDto dto, int userId)
    {
        if (dto.Details.Count == 0) throw new ArgumentException("A package must contain at least one item.");

        var code = dto.PackageCode;
        if (string.IsNullOrWhiteSpace(code))
        {
            var count = await _db.PackageMasters.IgnoreQueryFilters().CountAsync();
            code = $"PKG-{(count + 1):D5}";
        }
        else if (await _db.PackageMasters.AnyAsync(p => p.PackageCode == code))
        {
            throw new InvalidOperationException($"Package code '{code}' already exists.");
        }

        await ValidateItemsExistAsync(dto.Details.Select(d => d.ItemId));

        var entity = new PackageMaster
        {
            PackageCode = code,
            PackageName = dto.PackageName,
            PackagePrice = dto.PackagePrice,
            Description = dto.Description,
            IsActive = true,
            CreatedBy = userId,
            Details = dto.Details.Select(d => new PackageDetail { ItemId = d.ItemId, Quantity = d.Quantity }).ToList()
        };
        _db.PackageMasters.Add(entity);
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(entity.PackageId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdatePackageDto dto, int userId)
    {
        var entity = await _db.PackageMasters.Include(p => p.Details).FirstOrDefaultAsync(p => p.PackageId == id);
        if (entity is null) return false;
        if (dto.Details.Count == 0) throw new ArgumentException("A package must contain at least one item.");

        await ValidateItemsExistAsync(dto.Details.Select(d => d.ItemId));

        entity.PackageName = dto.PackageName;
        entity.PackagePrice = dto.PackagePrice;
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;

        _db.PackageDetails.RemoveRange(entity.Details);
        entity.Details = dto.Details.Select(d => new PackageDetail { ItemId = d.ItemId, Quantity = d.Quantity }).ToList();

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _db.PackageMasters.FirstOrDefaultAsync(p => p.PackageId == id);
        if (entity is null) return false;
        if (await _db.SalesDetails.AnyAsync(d => d.PackageId == id))
            throw new InvalidOperationException("Cannot delete a package that has sales history — deactivate it instead.");
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task ValidateItemsExistAsync(IEnumerable<int> itemIds)
    {
        var ids = itemIds.Distinct().ToList();
        var found = await _db.ItemMasters.Where(i => ids.Contains(i.ItemId)).CountAsync();
        if (found != ids.Count) throw new ArgumentException("One or more package items were not found.");
    }

    private static PackageDto MapToDto(PackageMaster p) => new()
    {
        PackageId = p.PackageId,
        PackageCode = p.PackageCode,
        PackageName = p.PackageName,
        PackagePrice = p.PackagePrice,
        Description = p.Description,
        IsActive = p.IsActive,
        Details = p.Details.Select(d => new PackageDetailDto
        {
            PackageDetailId = d.PackageDetailId,
            ItemId = d.ItemId,
            ItemName = d.Item?.ItemName ?? string.Empty,
            Barcode = d.Item?.Barcode,
            Quantity = d.Quantity,
            ItemSalePrice = d.Item?.SalePrice ?? 0m
        }).ToList()
    };
}
