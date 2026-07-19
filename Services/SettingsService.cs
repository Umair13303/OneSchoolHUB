using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Settings;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;
using System.Text.Json;

namespace SchoolManagement.API.Services;

public interface ISettingsService
{
    Task<SchoolSettingsDto> GetAsync();
    Task<SchoolSettingsDto> SaveAsync(UpdateSchoolSettingsDto dto, int updatedBy);
}

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _db;
    private readonly ITenantContext _tenant;
    public SettingsService(AppDbContext db, ITenantContext tenant) { _db = db; _tenant = tenant; }

    public async Task<SchoolSettingsDto> GetAsync()
    {
        var s = await GetOrCreateAsync();
        return ToDto(s);
    }

    public async Task<SchoolSettingsDto> SaveAsync(UpdateSchoolSettingsDto dto, int updatedBy)
    {
        var s = await GetOrCreateAsync();

        s.RegularStartTime    = TimeOnly.Parse(dto.RegularStartTime);
        s.RegularEndTime      = TimeOnly.Parse(dto.RegularEndTime);
        s.RegularTotalPeriods = dto.RegularTotalPeriods;
        s.RegularBreakAllowed = dto.RegularBreakAllowed;
        s.RegularBreakStart   = TimeOnly.Parse(dto.RegularBreakStart);
        s.RegularBreakEnd     = TimeOnly.Parse(dto.RegularBreakEnd);
        s.RegularGapMinutes   = dto.RegularGapMinutes;

        s.FridayStartTime    = TimeOnly.Parse(dto.FridayStartTime);
        s.FridayEndTime      = TimeOnly.Parse(dto.FridayEndTime);
        s.FridayTotalPeriods = dto.FridayTotalPeriods;
        s.FridayBreakAllowed = dto.FridayBreakAllowed;
        s.FridayBreakStart   = TimeOnly.Parse(dto.FridayBreakStart);
        s.FridayBreakEnd     = TimeOnly.Parse(dto.FridayBreakEnd);
        s.FridayGapMinutes   = dto.FridayGapMinutes;

        s.SundayEnabled   = dto.SundayEnabled;
        s.SundayStartTime = TimeOnly.Parse(dto.SundayStartTime);
        s.SundayEndTime   = TimeOnly.Parse(dto.SundayEndTime);

        s.WorkingDaysJson          = JsonSerializer.Serialize(dto.WorkingDays ?? new List<int> { 1, 2, 3, 4, 6 });
        s.RegularBreakAfterPeriod  = dto.RegularBreakAfterPeriod;
        s.FridayBreakAfterPeriod   = dto.FridayBreakAfterPeriod;
        s.RegularBreakDuration     = dto.RegularBreakDuration;
        s.FridayBreakDuration      = dto.FridayBreakDuration;
        s.HolidaysJson = JsonSerializer.Serialize(dto.Holidays ?? new List<string>());

        s.MinPeriodDurationMinutes = dto.MinPeriodDurationMinutes;
        s.MaxPeriodDurationMinutes = dto.MaxPeriodDurationMinutes;
        s.MaxPeriodsPerDay         = dto.MaxPeriodsPerDay;
        s.AppName   = dto.AppName ?? "Dev_Soloutions";

        s.AdmissionNoPrefix      = string.IsNullOrWhiteSpace(dto.AdmissionNoPrefix)
                                     ? "ADM" : dto.AdmissionNoPrefix.Trim().ToUpperInvariant();
        s.AdmissionNoIncludeYear = dto.AdmissionNoIncludeYear;
        s.AdmissionNoPadding     = dto.AdmissionNoPadding;
        s.UpdatedAt = DateTime.UtcNow;
        s.UpdatedBy = updatedBy;

        await _db.SaveChangesAsync();
        return ToDto(s);
    }

    // ── helpers ──────────────────────────────────────────────────────────────
    private async Task<SchoolSettings> GetOrCreateAsync()
    {
        // Each institute has its own settings row. The global query filter on
        // SchoolSettings already scopes by InstituteId, so FirstOrDefault finds
        // the right one. For superadmin with no institute, fall back to Id=1.
        // A campus can have its own override row (CampusId set): prefer the row
        // matching the user's campus, then the institute-wide row (CampusId null).
        var campusId = _tenant.CampusId;
        var s = await _db.SchoolSettings
            .OrderByDescending(x => campusId != null && x.CampusId == campusId)
            .ThenBy(x => x.CampusId != null)
            .FirstOrDefaultAsync();
        if (s is not null) return s;

        s = new SchoolSettings();
        // InstituteId will be stamped by TenantStampInterceptor for non-superadmins.
        // For superadmin (no institute), leave null so it acts as the global default.
        _db.SchoolSettings.Add(s);
        await _db.SaveChangesAsync();
        return s;
    }

    private static SchoolSettingsDto ToDto(SchoolSettings s)
    {
        List<int> workingDays = new() { 1, 2, 3, 4, 6 };
        try { workingDays = JsonSerializer.Deserialize<List<int>>(s.WorkingDaysJson) ?? workingDays; } catch { }

        List<string> holidays = new();
        try { holidays = JsonSerializer.Deserialize<List<string>>(s.HolidaysJson) ?? new(); } catch { }

        return new SchoolSettingsDto
        {
            RegularStartTime    = s.RegularStartTime.ToString("HH:mm"),
            RegularEndTime      = s.RegularEndTime.ToString("HH:mm"),
            RegularTotalPeriods = s.RegularTotalPeriods,
            RegularBreakAllowed = s.RegularBreakAllowed,
            RegularBreakStart   = s.RegularBreakStart.ToString("HH:mm"),
            RegularBreakEnd     = s.RegularBreakEnd.ToString("HH:mm"),
            RegularGapMinutes   = s.RegularGapMinutes,

            FridayStartTime    = s.FridayStartTime.ToString("HH:mm"),
            FridayEndTime      = s.FridayEndTime.ToString("HH:mm"),
            FridayTotalPeriods = s.FridayTotalPeriods,
            FridayBreakAllowed = s.FridayBreakAllowed,
            FridayBreakStart   = s.FridayBreakStart.ToString("HH:mm"),
            FridayBreakEnd     = s.FridayBreakEnd.ToString("HH:mm"),
            FridayGapMinutes   = s.FridayGapMinutes,

            SundayEnabled   = s.SundayEnabled,
            SundayStartTime = s.SundayStartTime.ToString("HH:mm"),
            SundayEndTime   = s.SundayEndTime.ToString("HH:mm"),

            WorkingDays = workingDays,
            RegularBreakAfterPeriod = s.RegularBreakAfterPeriod,
            FridayBreakAfterPeriod  = s.FridayBreakAfterPeriod,
            RegularBreakDuration    = s.RegularBreakDuration,
            FridayBreakDuration     = s.FridayBreakDuration,
            Holidays = holidays,

            MinPeriodDurationMinutes = s.MinPeriodDurationMinutes,
            MaxPeriodDurationMinutes = s.MaxPeriodDurationMinutes,
            MaxPeriodsPerDay         = s.MaxPeriodsPerDay,
            AppName                  = s.AppName,

            AdmissionNoPrefix      = s.AdmissionNoPrefix,
            AdmissionNoIncludeYear = s.AdmissionNoIncludeYear,
            AdmissionNoPadding     = s.AdmissionNoPadding,
        };
    }
}
