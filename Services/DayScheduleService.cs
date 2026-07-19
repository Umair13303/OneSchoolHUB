using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Settings;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IDayScheduleService
{
    Task<List<DayScheduleDto>> GetAllAsync();
    Task<List<DayScheduleDto>> BulkUpdateAsync(List<UpdateDayScheduleRequest> days);
}

public class DayScheduleService : IDayScheduleService
{
    private static readonly string[] DayNames = ["", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    private readonly AppDbContext _db;
    public DayScheduleService(AppDbContext db) => _db = db;

    public async Task<List<DayScheduleDto>> GetAllAsync()
    {
        await EnsureSeedAsync();
        var rows = await _db.DaySchedules.OrderBy(d => d.DayOfWeek).ToListAsync();
        return rows.Select(ToDto).ToList();
    }

    public async Task<List<DayScheduleDto>> BulkUpdateAsync(List<UpdateDayScheduleRequest> days)
    {
        await EnsureSeedAsync();
        foreach (var req in days)
        {
            var row = await _db.DaySchedules.FirstOrDefaultAsync(d => d.DayOfWeek == req.DayOfWeek);
            if (row is null) continue;
            row.IsWorkingDay     = req.IsWorkingDay;
            row.StartTime        = TimeOnly.Parse(req.StartTime);
            row.EndTime          = TimeOnly.Parse(req.EndTime);
            row.NumberOfPeriods  = req.NumberOfPeriods;
            row.HasBreak         = req.HasBreak;
            row.BreakAfterPeriod = req.BreakAfterPeriod;
            row.BreakDuration    = req.BreakDuration;
            row.UpdatedAt        = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync();
        return await GetAllAsync();
    }

    private async Task EnsureSeedAsync()
    {
        if (await _db.DaySchedules.AnyAsync()) return;

        var defaults = new[]
        {
            new DaySchedule { DayOfWeek = 1, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
            new DaySchedule { DayOfWeek = 2, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
            new DaySchedule { DayOfWeek = 3, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
            new DaySchedule { DayOfWeek = 4, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
            new DaySchedule { DayOfWeek = 5, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(11,30), NumberOfPeriods = 4 },
            new DaySchedule { DayOfWeek = 6, IsWorkingDay = true,  StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
            new DaySchedule { DayOfWeek = 7, IsWorkingDay = false, StartTime = new(8,0), EndTime = new(13,0), NumberOfPeriods = 6 },
        };
        _db.DaySchedules.AddRange(defaults);
        await _db.SaveChangesAsync();
    }

    private static DayScheduleDto ToDto(DaySchedule d) => new()
    {
        DayOfWeek        = d.DayOfWeek,
        DayName          = DayNames[d.DayOfWeek],
        IsWorkingDay     = d.IsWorkingDay,
        StartTime        = d.StartTime.ToString("HH:mm"),
        EndTime          = d.EndTime.ToString("HH:mm"),
        NumberOfPeriods  = d.NumberOfPeriods,
        HasBreak         = d.HasBreak,
        BreakAfterPeriod = d.BreakAfterPeriod,
        BreakDuration    = d.BreakDuration,
    };
}
