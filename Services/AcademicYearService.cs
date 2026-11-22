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
        await EnsureNoOverlapAsync(dto.StartDate, dto.EndDate, excludeId: null);

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

        await EnsureNoOverlapAsync(dto.StartDate, dto.EndDate, excludeId: id);

        var labelClash = await _db.AcademicYears
            .AnyAsync(y => y.YearLabel == dto.YearLabel && y.AcademicYearId != id && !y.IsDeleted);
        if (labelClash)
            throw new InvalidOperationException($"Another academic year already uses the label '{dto.YearLabel}'.");

        // Do not leave the school with zero active years.
        if (entity.IsActive && !dto.IsActive)
        {
            var otherActive = await _db.AcademicYears
                .AnyAsync(y => y.IsActive && !y.IsDeleted && y.AcademicYearId != id);
            if (!otherActive)
                throw new InvalidOperationException("Cannot deactivate the only active academic year. Activate another year first.");
        }

        entity.YearLabel = dto.YearLabel;
        entity.StartDate = dto.StartDate;
        entity.EndDate   = dto.EndDate;

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

        if (entity.IsActive)
        {
            var otherActive = await _db.AcademicYears
                .AnyAsync(y => y.IsActive && !y.IsDeleted && y.AcademicYearId != id);
            if (!otherActive)
                throw new InvalidOperationException("Cannot delete the only active academic year. Activate another year first.");
        }

        var inUse =
            await _db.StudentClassEnrollments.AnyAsync(e => e.AcademicYearId == id) ||
            await _db.SchoolTimetables.AnyAsync(t => t.AcademicYearId == id && !t.IsDeleted) ||
            await _db.AcademicCalendarEvents.AnyAsync(e => e.AcademicYearId == id && !e.IsDeleted) ||
            await _db.CoursePlans.AnyAsync(p => p.AcademicYearId == id && !p.IsDeleted);

        if (inUse)
            throw new InvalidOperationException("Cannot delete an academic year that has enrollments, timetable, calendar events, or course plans.");

        entity.IsDeleted = true;
        entity.IsActive  = false;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task DeactivateOthersAsync()
    {
        await _db.AcademicYears
            .Where(y => y.IsActive && !y.IsDeleted)
            .ExecuteUpdateAsync(s => s.SetProperty(y => y.IsActive, false));
    }

    private async Task EnsureNoOverlapAsync(DateOnly start, DateOnly end, int? excludeId)
    {
        var overlap = await _db.AcademicYears.AnyAsync(y =>
            !y.IsDeleted &&
            (!excludeId.HasValue || y.AcademicYearId != excludeId.Value) &&
            y.StartDate <= end &&
            y.EndDate >= start);
        if (overlap)
            throw new InvalidOperationException("Academic year dates overlap an existing year.");
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
