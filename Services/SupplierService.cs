using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ISupplierService
{
    Task<List<SupplierDto>> GetAllAsync(string? search, bool activeOnly = false);
    Task<SupplierDto?> GetByIdAsync(int id);
    Task<SupplierDto> CreateAsync(SaveSupplierDto dto, int userId);
    Task<bool> UpdateAsync(int id, SaveSupplierDto dto, int userId);
    Task<bool> DeleteAsync(int id, int userId);
}

public class SupplierService : ISupplierService
{
    private readonly AppDbContext _db;
    public SupplierService(AppDbContext db) => _db = db;

    public async Task<List<SupplierDto>> GetAllAsync(string? search, bool activeOnly = false)
    {
        var q = _db.Suppliers.AsNoTracking().AsQueryable();
        if (activeOnly) q = q.Where(s => s.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(s => s.SupplierName.Contains(search) || (s.Mobile != null && s.Mobile.Contains(search)));
        var list = await q.OrderBy(s => s.SupplierName).ToListAsync();
        return list.Select(MapToDto).ToList();
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.SupplierId == id);
        return entity is null ? null : MapToDto(entity);
    }

    public async Task<SupplierDto> CreateAsync(SaveSupplierDto dto, int userId)
    {
        var entity = new Supplier
        {
            SupplierName = dto.SupplierName,
            ContactPerson = dto.ContactPerson,
            Mobile = dto.Mobile,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            City = dto.City,
            IsActive = dto.IsActive,
            CreatedBy = userId
        };
        _db.Suppliers.Add(entity);
        await _db.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, SaveSupplierDto dto, int userId)
    {
        var entity = await _db.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
        if (entity is null) return false;
        entity.SupplierName = dto.SupplierName;
        entity.ContactPerson = dto.ContactPerson;
        entity.Mobile = dto.Mobile;
        entity.Phone = dto.Phone;
        entity.Email = dto.Email;
        entity.Address = dto.Address;
        entity.City = dto.City;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var entity = await _db.Suppliers.FirstOrDefaultAsync(s => s.SupplierId == id);
        if (entity is null) return false;
        if (await _db.PurchaseMasters.AnyAsync(p => p.SupplierId == id))
            throw new InvalidOperationException("Cannot delete a supplier that has purchase history — deactivate it instead.");
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private static SupplierDto MapToDto(Supplier s) => new()
    {
        SupplierId = s.SupplierId,
        SupplierName = s.SupplierName,
        ContactPerson = s.ContactPerson,
        Mobile = s.Mobile,
        Phone = s.Phone,
        Email = s.Email,
        Address = s.Address,
        City = s.City,
        IsActive = s.IsActive
    };
}
