using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>Simple lookup data used across the module: item categories, units of measure, tax rates.</summary>
public interface IInventoryMasterService
{
    Task<List<ItemCategoryDto>> GetCategoriesAsync();
    Task<ItemCategoryDto> CreateCategoryAsync(SaveItemCategoryDto dto, int userId);
    Task<bool> UpdateCategoryAsync(int id, SaveItemCategoryDto dto, int userId);
    Task<bool> DeleteCategoryAsync(int id, int userId);

    Task<List<UnitDto>> GetUnitsAsync();
    Task<UnitDto> CreateUnitAsync(SaveUnitDto dto, int userId);
    Task<bool> UpdateUnitAsync(int id, SaveUnitDto dto, int userId);
    Task<bool> DeleteUnitAsync(int id, int userId);

    Task<List<TaxDto>> GetTaxesAsync();
    Task<TaxDto> CreateTaxAsync(SaveTaxDto dto, int userId);
    Task<bool> UpdateTaxAsync(int id, SaveTaxDto dto, int userId);
    Task<bool> DeleteTaxAsync(int id, int userId);
}

public class InventoryMasterService : IInventoryMasterService
{
    private readonly AppDbContext _db;
    public InventoryMasterService(AppDbContext db) => _db = db;

    // ── Categories ───────────────────────────────────────────────────────────
    public async Task<List<ItemCategoryDto>> GetCategoriesAsync()
        => await _db.ItemCategories.AsNoTracking()
            .OrderBy(c => c.CategoryName)
            .Select(c => new ItemCategoryDto
            {
                ItemCategoryId = c.ItemCategoryId,
                CategoryName = c.CategoryName,
                IsActive = c.IsActive,
                ItemCount = c.Items.Count
            }).ToListAsync();

    public async Task<ItemCategoryDto> CreateCategoryAsync(SaveItemCategoryDto dto, int userId)
    {
        var entity = new ItemCategory { CategoryName = dto.CategoryName, IsActive = dto.IsActive, CreatedBy = userId };
        _db.ItemCategories.Add(entity);
        await _db.SaveChangesAsync();
        return new ItemCategoryDto { ItemCategoryId = entity.ItemCategoryId, CategoryName = entity.CategoryName, IsActive = entity.IsActive };
    }

    public async Task<bool> UpdateCategoryAsync(int id, SaveItemCategoryDto dto, int userId)
    {
        var entity = await _db.ItemCategories.FirstOrDefaultAsync(c => c.ItemCategoryId == id);
        if (entity is null) return false;
        entity.CategoryName = dto.CategoryName;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCategoryAsync(int id, int userId)
    {
        var entity = await _db.ItemCategories.FirstOrDefaultAsync(c => c.ItemCategoryId == id);
        if (entity is null) return false;
        if (await _db.ItemMasters.AnyAsync(i => i.ItemCategoryId == id))
            throw new InvalidOperationException("Cannot delete a category that has items assigned.");
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Units ────────────────────────────────────────────────────────────────
    public async Task<List<UnitDto>> GetUnitsAsync()
        => await _db.UnitMasters.AsNoTracking().OrderBy(u => u.UnitName)
            .Select(u => new UnitDto { UnitId = u.UnitId, UnitName = u.UnitName, ShortName = u.ShortName, IsActive = u.IsActive })
            .ToListAsync();

    public async Task<UnitDto> CreateUnitAsync(SaveUnitDto dto, int userId)
    {
        var entity = new UnitMaster { UnitName = dto.UnitName, ShortName = dto.ShortName, IsActive = dto.IsActive, CreatedBy = userId };
        _db.UnitMasters.Add(entity);
        await _db.SaveChangesAsync();
        return new UnitDto { UnitId = entity.UnitId, UnitName = entity.UnitName, ShortName = entity.ShortName, IsActive = entity.IsActive };
    }

    public async Task<bool> UpdateUnitAsync(int id, SaveUnitDto dto, int userId)
    {
        var entity = await _db.UnitMasters.FirstOrDefaultAsync(u => u.UnitId == id);
        if (entity is null) return false;
        entity.UnitName = dto.UnitName;
        entity.ShortName = dto.ShortName;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUnitAsync(int id, int userId)
    {
        var entity = await _db.UnitMasters.FirstOrDefaultAsync(u => u.UnitId == id);
        if (entity is null) return false;
        if (await _db.ItemMasters.AnyAsync(i => i.UnitId == id))
            throw new InvalidOperationException("Cannot delete a unit that has items assigned.");
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Tax ──────────────────────────────────────────────────────────────────
    public async Task<List<TaxDto>> GetTaxesAsync()
        => await _db.TaxMasters.AsNoTracking().OrderBy(t => t.TaxName)
            .Select(t => new TaxDto { TaxId = t.TaxId, TaxName = t.TaxName, TaxPercentage = t.TaxPercentage, IsActive = t.IsActive })
            .ToListAsync();

    public async Task<TaxDto> CreateTaxAsync(SaveTaxDto dto, int userId)
    {
        var entity = new TaxMaster { TaxName = dto.TaxName, TaxPercentage = dto.TaxPercentage, IsActive = dto.IsActive, CreatedBy = userId };
        _db.TaxMasters.Add(entity);
        await _db.SaveChangesAsync();
        return new TaxDto { TaxId = entity.TaxId, TaxName = entity.TaxName, TaxPercentage = entity.TaxPercentage, IsActive = entity.IsActive };
    }

    public async Task<bool> UpdateTaxAsync(int id, SaveTaxDto dto, int userId)
    {
        var entity = await _db.TaxMasters.FirstOrDefaultAsync(t => t.TaxId == id);
        if (entity is null) return false;
        entity.TaxName = dto.TaxName;
        entity.TaxPercentage = dto.TaxPercentage;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTaxAsync(int id, int userId)
    {
        var entity = await _db.TaxMasters.FirstOrDefaultAsync(t => t.TaxId == id);
        if (entity is null) return false;
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }
}
