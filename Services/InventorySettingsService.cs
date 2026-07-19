using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>SRS §17 configuration — one row per institute, created lazily on first read.</summary>
public interface IInventorySettingsService
{
    Task<InventorySettingsDto> GetAsync();
    Task<InventorySettingsDto> UpdateAsync(UpdateInventorySettingsDto dto, int userId);
}

public class InventorySettingsService : IInventorySettingsService
{
    private readonly AppDbContext _db;
    public InventorySettingsService(AppDbContext db) => _db = db;

    public async Task<InventorySettingsDto> GetAsync()
    {
        var entity = await _db.InventorySettings.FirstOrDefaultAsync();
        if (entity is null)
        {
            entity = new InventorySettings();
            _db.InventorySettings.Add(entity);
            await _db.SaveChangesAsync();
        }
        return MapToDto(entity);
    }

    public async Task<InventorySettingsDto> UpdateAsync(UpdateInventorySettingsDto dto, int userId)
    {
        var entity = await _db.InventorySettings.FirstOrDefaultAsync();
        if (entity is null)
        {
            entity = new InventorySettings { CreatedBy = userId };
            _db.InventorySettings.Add(entity);
        }

        entity.CostingMethod = dto.CostingMethod;
        entity.BarcodeEnabled = dto.BarcodeEnabled;
        entity.PackageEnabled = dto.PackageEnabled;
        entity.StudentSalesEnabled = dto.StudentSalesEnabled;
        entity.NegativeStockAllowed = dto.NegativeStockAllowed;
        entity.AutoInvoiceNumber = dto.AutoInvoiceNumber;
        entity.AutoBarcode = dto.AutoBarcode;
        entity.DefaultTaxPercentage = dto.DefaultTaxPercentage;
        entity.DefaultCurrency = dto.DefaultCurrency;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto(entity);
    }

    private static InventorySettingsDto MapToDto(InventorySettings s) => new()
    {
        InventorySettingsId = s.InventorySettingsId,
        CostingMethod = s.CostingMethod,
        BarcodeEnabled = s.BarcodeEnabled,
        PackageEnabled = s.PackageEnabled,
        StudentSalesEnabled = s.StudentSalesEnabled,
        NegativeStockAllowed = s.NegativeStockAllowed,
        AutoInvoiceNumber = s.AutoInvoiceNumber,
        AutoBarcode = s.AutoBarcode,
        DefaultTaxPercentage = s.DefaultTaxPercentage,
        DefaultCurrency = s.DefaultCurrency
    };
}
