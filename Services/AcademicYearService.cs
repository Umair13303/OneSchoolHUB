using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IAcademicYearService
{
    Task<List<AcademicYearDto>> GetAllAsync();
    Task<AcademicYearDto?>      GetByIdAsync(int id);
    Task<AcademicYearDto?>      GetActiveAsync();
    Task<AcademicYearDto>       CreateAsync(CreateAcademicYearDto dto, int createdBy);
    Task<bool>                  UpdateAsync(int id, UpdateAcademicYearDto dto, int updatedBy);
    Task<bool>                  SetActiveAsync(int id, int updatedBy);
    Task<bool>                  DeleteAsync(int id, int deletedBy);
}

public class AcademicYearService : IAcademicYearService
{
    private readonly AppDbContext _db;

    public AcademicYearService(AppDbContext db) => _db = db;

    // AppDbContext does not register a HasQueryFilter for AcademicYear, so we
    // filter IsDeleted manually in every read.

    public async Task<List<AcademicYearDto>> GetAllAsync()
        => await _db.AcademicYears
            .AsNoTracking()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.StartDate)
            .Select(y => Map(y))
            .ToListAsync();

    public async Task<AcademicYearDto?> GetByIdAsync(int id)
        => await _db.AcademicYears
            .AsNoTracking()
            .Where(y => y.AcademicYearId == id && !y.IsDeleted)
            .Select(y => Map(y))
            .FirstOrDefaultAsync();

    public async Task<AcademicYearDto?> GetActiveAsync()
        => await _db.AcademicYears
            .AsNoTracking()
            .Where(y => y.IsActive && !y.IsDeleted)
            .OrderByDescending(y => y.StartDate)
            .Select(y => Map(y))
            .FirstOrDefaultAsync();

    public async Task<AcademicYearDto> CreateAsync(CreateAcademicYearDto dto, int createdBy)
    {
        ValidateDates(dto.StartDate, dto.EndDate);

        var labelExists = await _db.AcademicYears
            .AnyAsync(y => y.YearLabel == dto.YearLabel && !y.IsDeleted);
        if (labelExists)
            throw new InvalidOperationException($"An academic year with the label '{dto.YearLabel}' already exists.");

        var entity = new AcademicYear
        {
            YearLabel = dto.YearLabel,
            StartDate = dto.StartDate,
            EndDate   = dto.EndDate,
            IsActive  = dto.IsActive,
            CreatedBy = createdBy
        };

        if (dto.IsActive)
            await DeactivateOthersAsync();

        _db.AcademicYears.Add(entity);
        await _db.SaveChangesAsync();

        return Map(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAcademicYearDto dto, int updatedBy)
    {
        ValidateDates(dto.StartDate, dto.EndDate);

        var entity = await _db.AcademicYears.FirstOrDefaultAsync(y => y.AcademicYearId == id && !y.IsDeleted);
        if (entity is null) return false;

        var labelClash = await _db.AcademicYears
            .AnyAsync(y => y.YearLabel == dto.YearLabel && y.AcademicYearId != id && !y.IsDeleted);
        if (labelClash)
            throw new InvalidOperationException($"Another academic year already uses the label '{dto.YearLabel}'.");

        entity.YearLabel = dto.YearLabel;
        entity.StartDate = dto.StartDate;
        entity.EndDate   = dto.EndDate;

        // If the caller flipped IsActive from false→true, deactivate others.
        if (dto.IsActive && !entity.IsActive)
            await DeactivateOthersAsync();
        entity.IsActive  = dto.IsActive;

        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SetActiveAsync(int id, int updatedBy)
    {
        var entity = await _db.AcademicYears.FirstOrDefaultAsync(y => y.AcademicYearId == id && !y.IsDeleted);
        if (entity is null) return false;

        await DeactivateOthersAsync();
        entity.IsActive  = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int deletedBy)
    {
        var entity = await _db.AcademicYears.FirstOrDefaultAsync(y => y.AcademicYearId == id && !y.IsDeleted);
        if (entity is null) return false;


        entity.IsDeleted = true;
        entity.IsActive  = false;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ──────────────────────────────────────────────────────────────────────
    private async Task DeactivateOthersAsync()
    {
        await _db.AcademicYears
            .Where(y => y.IsActive)
            .ExecuteUpdateAsync(s => s.SetProperty(y => y.IsActive, false));
    }

    private static void ValidateDates(DateOnly start, DateOnly end)
    {
        if (end <= start)
            throw new ArgumentException("EndDate must be after StartDate.");
    }

    private static AcademicYearDto Map(AcademicYear y) => new()
    {
        AcademicYearId = y.AcademicYearId,
        YearLabel      = y.YearLabel,
        StartDate      = y.StartDate,
        EndDate        = y.EndDate,
        IsActive       = y.IsActive
    };
}
