using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Settings;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IScheduleProfileService
{
    Task<List<ScheduleProfileDto>> GetAllAsync();
    Task<ScheduleProfileDto>       GetAsync(int id);
    Task<ScheduleProfileDto>       CreateAsync(CreateScheduleProfileDto dto);
    Task<ScheduleProfileDto>       CopyAsync(int id, string name);
    Task<ScheduleProfileDto>       RenameAsync(int id, RenameScheduleProfileDto dto);
    Task<ScheduleProfileDto>       UpdateDaysAsync(int id, List<UpdateDayScheduleRequest> days);
    Task<ScheduleProfileDto>       ActivateAsync(int id);
    Task                           DeleteAsync(int id);
    Task<List<ProfilePeriodDto>>   GetPeriodsAsync(int id);
    Task<List<ProfilePeriodDto>>   SavePeriodsAsync(int id, List<ProfilePeriodDto> periods);
    Task<List<ProfilePeriodDto>>   SyncPeriodsFromActiveProfileAsync();
    Task<Dictionary<int, List<ProfilePeriodDto>>> GetActiveProfilePeriodsPerDayAsync();
}

public class ScheduleProfileService : IScheduleProfileService
{
    private static readonly string[] DayNames = ["", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    private readonly AppDbContext _db;
    public ScheduleProfileService(AppDbContext db) => _db = db;

    public async Task<List<ScheduleProfileDto>> GetAllAsync()
    {
        await EnsureDefaultAsync();
        return await _db.ScheduleProfiles
            .Include(p => p.Days)
            .OrderBy(p => p.CreatedAt)
            .Select(p => ToDto(p))
            .ToListAsync();
    }

    public async Task<ScheduleProfileDto> GetAsync(int id)
    {
        var p = await FindAsync(id);
        return ToDto(p);
    }

    public async Task<ScheduleProfileDto> CreateAsync(CreateScheduleProfileDto dto)
    {
        var profile = new ScheduleProfile { Name = dto.Name.Trim(), IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        profile.Days = DefaultDays();
        _db.ScheduleProfiles.Add(profile);
        await _db.SaveChangesAsync();
        return ToDto(profile);
    }

    public async Task<ScheduleProfileDto> CopyAsync(int id, string name)
    {
        var src = await FindAsync(id);
        var copy = new ScheduleProfile { Name = name.Trim(), IsActive = false, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        copy.Days = src.Days.Select(d => new DaySchedule
        {
            DayOfWeek = d.DayOfWeek, IsWorkingDay = d.IsWorkingDay,
            StartTime = d.StartTime, EndTime = d.EndTime,
            NumberOfPeriods = d.NumberOfPeriods, HasBreak = d.HasBreak,
            BreakAfterPeriod = d.BreakAfterPeriod, BreakDuration = d.BreakDuration,
        }).ToList();
        _db.ScheduleProfiles.Add(copy);
        await _db.SaveChangesAsync();
        return ToDto(copy);
    }

    public async Task<ScheduleProfileDto> RenameAsync(int id, RenameScheduleProfileDto dto)
    {
        var p = await FindAsync(id);
        p.Name = dto.Name.Trim();
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ToDto(p);
    }

    public async Task<ScheduleProfileDto> UpdateDaysAsync(int id, List<UpdateDayScheduleRequest> days)
    {
        var p = await FindAsync(id);
        foreach (var req in days)
        {
            var row = p.Days.FirstOrDefault(d => d.DayOfWeek == req.DayOfWeek);
            if (row is null) continue;
            row.IsWorkingDay = req.IsWorkingDay; row.StartTime = TimeOnly.Parse(req.StartTime);
            row.EndTime = TimeOnly.Parse(req.EndTime); row.NumberOfPeriods = req.NumberOfPeriods;
            row.HasBreak = req.HasBreak; row.BreakAfterPeriod = req.BreakAfterPeriod;
            row.BreakDuration = req.BreakDuration; row.UpdatedAt = DateTime.UtcNow;
        }
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return ToDto(p);
    }

    public async Task<ScheduleProfileDto> ActivateAsync(int id)
    {
        await _db.ScheduleProfiles.ExecuteUpdateAsync(s => s.SetProperty(p => p.IsActive, false));
        var p = await FindAsync(id);
        p.IsActive = true;
        p.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        // Keep Periods table in sync with the newly active profile
        await SyncPeriodsFromActiveProfileAsync();
        return ToDto(p);
    }

    public async Task<Dictionary<int, List<ProfilePeriodDto>>> GetActiveProfilePeriodsPerDayAsync()
    {
        var active = await _db.ScheduleProfiles.FirstOrDefaultAsync(p => p.IsActive);
        if (active is null) return new();

        var rows = await _db.ProfilePeriods
            .Where(p => p.ProfileId == active.Id)
            .OrderBy(p => p.DayOfWeek).ThenBy(p => p.SortOrder)
            .ToListAsync();

        // Also fetch synced Periods (with their PeriodIds) ordered by PeriodId
        var dbPeriods = await _db.Periods
            .Where(p => !p.IsDeleted)
            .OrderBy(p => p.PeriodId)
            .ToListAsync();

        // Build the canonical day (most periods) for PeriodId mapping
        var canonicalDay = rows.GroupBy(p => p.DayOfWeek)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? 1;

        var canonicalRows = rows.Where(p => p.DayOfWeek == canonicalDay)
                                .OrderBy(p => p.SortOrder).ToList();

        // Map profile slot index → PeriodId
        var slotToPeriodId = new Dictionary<int, int>();
        for (int i = 0; i < canonicalRows.Count && i < dbPeriods.Count; i++)
            slotToPeriodId[i] = dbPeriods[i].PeriodId;

        var result = new Dictionary<int, List<ProfilePeriodDto>>();

        foreach (var dayGroup in rows.GroupBy(p => p.DayOfWeek))
        {
            var dayRows = dayGroup.OrderBy(p => p.SortOrder).ToList();
            var dtos = new List<ProfilePeriodDto>();
            for (int i = 0; i < dayRows.Count; i++)
            {
                var pp = dayRows[i];
                // Find the matching canonical slot index by SortOrder position
                var canonicalIdx = canonicalRows.FindIndex(c => c.SortOrder == pp.SortOrder);
                var periodId = canonicalIdx >= 0 && slotToPeriodId.TryGetValue(canonicalIdx, out var pid) ? pid : 0;
                dtos.Add(new ProfilePeriodDto
                {
                    DayOfWeek       = pp.DayOfWeek,
                    DayName         = DayNamesArr[pp.DayOfWeek],
                    PeriodNo        = pp.PeriodNo,
                    PeriodName      = pp.PeriodName,
                    StartTime       = pp.StartTime.ToString("HH:mm"),
                    EndTime         = pp.EndTime.ToString("HH:mm"),
                    DurationMinutes = pp.DurationMinutes,
                    IsBreak         = pp.IsBreak,
                    SortOrder       = pp.SortOrder,
                    PeriodId        = periodId,
                });
            }
            result[dayGroup.Key] = dtos;
        }

        return result;
    }

    public async Task<List<ProfilePeriodDto>> SyncPeriodsFromActiveProfileAsync()
    {
        var active = await _db.ScheduleProfiles.FirstOrDefaultAsync(p => p.IsActive);
        if (active is null) return [];

        // Use the day with the most periods as canonical (covers all possible slots).
        var profilePeriods = await _db.ProfilePeriods
            .Where(p => p.ProfileId == active.Id)
            .OrderBy(p => p.DayOfWeek).ThenBy(p => p.SortOrder)
            .ToListAsync();

        if (!profilePeriods.Any()) return [];

        // Pick the day that has the most period rows (longest schedule)
        var canonicalDay = profilePeriods
            .GroupBy(p => p.DayOfWeek)
            .OrderByDescending(g => g.Count())
            .First().Key;

        var day1 = profilePeriods.Where(p => p.DayOfWeek == canonicalDay)
                                  .OrderBy(p => p.SortOrder).ToList();
        if (!day1.Any()) return [];

        // Upsert Periods table to match the profile's canonical day.
        // Both lists must be ordered the same way (by slot position = PeriodId insertion order)
        // so positional mapping is correct. Do NOT sort by PeriodNo — break has PeriodNo=0
        // which would sort it to the top regardless of where it sits in the schedule.
        var existing = await _db.Periods
            .OrderBy(p => p.PeriodId)
            .ToListAsync();

        var result = new List<Period>();

        for (int i = 0; i < day1.Count; i++)
        {
            var pp = day1[i];
            if (i < existing.Count)
            {
                // Update the existing row in-place (preserves PeriodId FK references)
                var row = existing[i];
                row.PeriodNo   = pp.PeriodNo;
                row.PeriodName = pp.PeriodName;
                row.StartTime  = pp.StartTime;
                row.EndTime    = pp.EndTime;
                row.IsBreak    = pp.IsBreak;
                row.IsActive   = true;
                row.IsDeleted  = false;
                result.Add(row);
            }
            else
            {
                // Profile has more periods than DB rows — add new
                var row = new Period
                {
                    PeriodNo   = pp.PeriodNo,
                    PeriodName = pp.PeriodName,
                    StartTime  = pp.StartTime,
                    EndTime    = pp.EndTime,
                    IsBreak    = pp.IsBreak,
                    IsActive   = true,
                    IsDeleted  = false,
                };
                _db.Periods.Add(row);
                result.Add(row);
            }
        }

        // Soft-delete any extra DB rows beyond what the profile defines
        for (int i = day1.Count; i < existing.Count; i++)
            existing[i].IsDeleted = true;

        await _db.SaveChangesAsync();

        return result.Select(p => new ProfilePeriodDto
        {
            PeriodNo        = p.PeriodNo,
            PeriodName      = p.PeriodName,
            StartTime       = p.StartTime.ToString("HH:mm"),
            EndTime         = p.EndTime.ToString("HH:mm"),
            DurationMinutes = (int)(p.EndTime - p.StartTime).TotalMinutes,
            IsBreak         = p.IsBreak,
        }).ToList();
    }

    public async Task DeleteAsync(int id)
    {
        var p = await FindAsync(id);
        if (p.IsActive) throw new InvalidOperationException("Cannot delete the active profile.");
        _db.ScheduleProfiles.Remove(p);
        await _db.SaveChangesAsync();
    }

    public async Task<List<ProfilePeriodDto>> GetPeriodsAsync(int id)
    {
        var rows = await _db.ProfilePeriods
            .Where(p => p.ProfileId == id)
            .OrderBy(p => p.DayOfWeek).ThenBy(p => p.SortOrder)
            .ToListAsync();
        return rows.Select(ToPeriodDto).ToList();
    }

    public async Task<List<ProfilePeriodDto>> SavePeriodsAsync(int id, List<ProfilePeriodDto> periods)
    {
        // replace all periods for this profile
        var existing = await _db.ProfilePeriods.Where(p => p.ProfileId == id).ToListAsync();
        _db.ProfilePeriods.RemoveRange(existing);
        var newRows = periods.Select((p, i) => new ProfilePeriod
        {
            ProfileId       = id,
            DayOfWeek       = p.DayOfWeek,
            PeriodNo        = p.PeriodNo,
            PeriodName      = p.PeriodName,
            StartTime       = TimeOnly.Parse(p.StartTime),
            EndTime         = TimeOnly.Parse(p.EndTime),
            DurationMinutes = p.DurationMinutes,
            IsBreak         = p.IsBreak,
            SortOrder       = p.SortOrder,
        }).ToList();
        _db.ProfilePeriods.AddRange(newRows);
        await _db.SaveChangesAsync();
        return newRows.Select(ToPeriodDto).ToList();
    }

    private static readonly string[] DayNamesArr = ["", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    private static ProfilePeriodDto ToPeriodDto(ProfilePeriod p) => new()
    {
        DayOfWeek       = p.DayOfWeek,
        DayName         = DayNamesArr[p.DayOfWeek],
        PeriodNo        = p.PeriodNo,
        PeriodName      = p.PeriodName,
        StartTime       = p.StartTime.ToString("HH:mm"),
        EndTime         = p.EndTime.ToString("HH:mm"),
        DurationMinutes = p.DurationMinutes,
        IsBreak         = p.IsBreak,
        SortOrder       = p.SortOrder,
    };

    // ── helpers ────────────────────────────────────────────────────────────────
    private async Task<ScheduleProfile> FindAsync(int id) =>
        await _db.ScheduleProfiles.Include(p => p.Days).FirstOrDefaultAsync(p => p.Id == id)
        ?? throw new KeyNotFoundException($"Profile {id} not found.");

    private async Task EnsureDefaultAsync()
    {
        if (await _db.ScheduleProfiles.AnyAsync()) return;
        var p = new ScheduleProfile { Name = "Default Schedule", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        p.Days = DefaultDays();
        _db.ScheduleProfiles.Add(p);
        await _db.SaveChangesAsync();
    }

    private static List<DaySchedule> DefaultDays() =>
    [
        new() { DayOfWeek = 1, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
        new() { DayOfWeek = 2, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
        new() { DayOfWeek = 3, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
        new() { DayOfWeek = 4, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
        new() { DayOfWeek = 5, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(11,30), NumberOfPeriods = 4 },
        new() { DayOfWeek = 6, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
        new() { DayOfWeek = 7, IsWorkingDay = false, StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
    ];

    private static ScheduleProfileDto ToDto(ScheduleProfile p) => new()
    {
        Id = p.Id, Name = p.Name, IsActive = p.IsActive, CreatedAt = p.CreatedAt,
        Days = p.Days.OrderBy(d => d.DayOfWeek).Select(d => new DayScheduleDto
        {
            DayOfWeek = d.DayOfWeek, DayName = DayNames[d.DayOfWeek],
            IsWorkingDay = d.IsWorkingDay, StartTime = d.StartTime.ToString("HH:mm"),
            EndTime = d.EndTime.ToString("HH:mm"), NumberOfPeriods = d.NumberOfPeriods,
            HasBreak = d.HasBreak, BreakAfterPeriod = d.BreakAfterPeriod, BreakDuration = d.BreakDuration,
        }).ToList(),
    };
}
