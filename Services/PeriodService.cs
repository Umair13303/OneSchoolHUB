using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Timetable;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IPeriodService
{
    Task<List<PeriodDto>> GetAllAsync();
    Task<PeriodDto?>      GetByIdAsync(int id);
    Task<PeriodDto>       CreateAsync(CreatePeriodDto dto, int createdBy);
    Task<bool>            UpdateAsync(int id, UpdatePeriodDto dto, int updatedBy);
    Task<bool>            SoftDeleteAsync(int id);
    Task<List<PeriodDto>> BulkReplaceAsync(List<UpsertPeriodDto> periods, int createdBy);
}

/// <summary>
/// CRUD for the daily period schedule. <see cref="Period"/> doesn't inherit
/// BaseEntity and has no <c>HasQueryFilter</c> in AppDbContext, so we filter
/// <c>IsDeleted</c> manually here.
/// </summary>
public class PeriodService : IPeriodService
{
    private readonly AppDbContext _db;
    public PeriodService(AppDbContext db) => _db = db;

    public async Task<List<PeriodDto>> GetAllAsync()
        => await _db.Periods.AsNoTracking()
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.PeriodId)
            .Select(p => Map(p))
            .ToListAsync();

    public async Task<PeriodDto?> GetByIdAsync(int id)
        => await _db.Periods.AsNoTracking()
            .Where(p => p.PeriodId == id && !p.IsDeleted)
            .Select(p => Map(p))
            .FirstOrDefaultAsync();

    public async Task<PeriodDto> CreateAsync(CreatePeriodDto dto, int createdBy)
    {
        ValidateTimes(dto.StartTime, dto.EndTime);
        await EnsureNoOverlapAsync(dto.StartTime, dto.EndTime, excludeId: null);

        var nameClash = await _db.Periods.AnyAsync(p =>
            !p.IsDeleted && (p.PeriodNo == dto.PeriodNo || p.PeriodName == dto.PeriodName));
        if (nameClash)
            throw new InvalidOperationException("Another period already uses this PeriodNo or PeriodName.");

        var entity = new Period
        {
            PeriodNo   = dto.PeriodNo,
            PeriodName = dto.PeriodName,
            StartTime  = dto.StartTime,
            EndTime    = dto.EndTime,
            IsBreak    = dto.IsBreak,
            IsActive   = dto.IsActive,
            CreatedBy  = createdBy
        };
        _db.Periods.Add(entity);
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<bool> UpdateAsync(int id, UpdatePeriodDto dto, int updatedBy)
    {
        ValidateTimes(dto.StartTime, dto.EndTime);

        var entity = await _db.Periods.FirstOrDefaultAsync(p => p.PeriodId == id && !p.IsDeleted);
        if (entity is null) return false;

        await EnsureNoOverlapAsync(dto.StartTime, dto.EndTime, excludeId: id);

        var nameClash = await _db.Periods.AnyAsync(p =>
            !p.IsDeleted && p.PeriodId != id &&
            (p.PeriodNo == dto.PeriodNo || p.PeriodName == dto.PeriodName));
        if (nameClash)
            throw new InvalidOperationException("Another period already uses this PeriodNo or PeriodName.");

        entity.PeriodNo   = dto.PeriodNo;
        entity.PeriodName = dto.PeriodName;
        entity.StartTime  = dto.StartTime;
        entity.EndTime    = dto.EndTime;
        entity.IsBreak    = dto.IsBreak;
        entity.IsActive   = dto.IsActive;
        // Period doesn't extend BaseEntity, so no UpdatedBy/UpdatedAt to set.
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await _db.Periods.FirstOrDefaultAsync(p => p.PeriodId == id && !p.IsDeleted);
        if (entity is null) return false;

        var inUse = await _db.SchoolTimetables.AnyAsync(t => t.PeriodId == id);
        if (inUse)
            throw new InvalidOperationException("Cannot delete a period that is referenced by the timetable.");

        entity.IsDeleted = true;
        entity.IsActive  = false;
        await _db.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Soft-delete all existing non-timetable-referenced periods and insert the new set.
    /// Periods still referenced by timetable entries are kept but de-activated.
    /// </summary>
    public async Task<List<PeriodDto>> BulkReplaceAsync(List<UpsertPeriodDto> periods, int createdBy)
    {
        // Soft-delete existing periods not in use
        var existing = await _db.Periods.Where(p => !p.IsDeleted).ToListAsync();
        var inUseIds = await _db.SchoolTimetables.Select(t => t.PeriodId).Distinct().ToListAsync();

        foreach (var p in existing)
        {
            p.IsDeleted = true;
            p.IsActive  = false;
        }
        await _db.SaveChangesAsync();

        // Insert new set
        var newEntities = periods.Select((p, i) => new Period
        {
            PeriodNo   = p.PeriodNo,
            PeriodName = p.PeriodName,
            StartTime  = TimeOnly.Parse(p.StartTime),
            EndTime    = TimeOnly.Parse(p.EndTime),
            IsBreak    = p.IsBreak,
            IsActive   = true,
            CreatedBy  = createdBy
        }).ToList();

        _db.Periods.AddRange(newEntities);
        await _db.SaveChangesAsync();

        return newEntities.Select(Map).ToList();
    }

    // ──────────────────────────────────────────────────────────────────────
    private static void ValidateTimes(TimeOnly start, TimeOnly end)
    {
        if (end <= start)
            throw new ArgumentException("EndTime must be after StartTime.");
    }

    /// <summary>Reject overlapping period times. Two periods that share an end-then-start boundary (e.g. 08:40-09:20 and 09:20-10:00) are allowed.</summary>
    private async Task EnsureNoOverlapAsync(TimeOnly start, TimeOnly end, int? excludeId)
    {
        var overlaps = await _db.Periods.AnyAsync(p =>
            !p.IsDeleted &&
            (!excludeId.HasValue || p.PeriodId != excludeId.Value) &&
            p.StartTime < end && start < p.EndTime);
        if (overlaps)
            throw new InvalidOperationException("Another period overlaps this time range.");
    }

    private static PeriodDto Map(Period p) => new()
    {
        PeriodId   = p.PeriodId,
        PeriodNo   = p.PeriodNo,
        PeriodName = p.PeriodName,
        StartTime  = p.StartTime,
        EndTime    = p.EndTime,
        IsBreak    = p.IsBreak,
        IsActive   = p.IsActive
    };
}
