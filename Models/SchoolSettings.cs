namespace SchoolManagement.API.Models;

/// <summary>
/// Singleton row (Id = 1) that stores all school-wide timing configuration.
/// Regular days = Mon/Tue/Wed/Thu/Sat. Friday has shorter timing. Sunday is off.
/// </summary>
public class SchoolSettings
{
    public int Id { get; set; } = 1;

    // ── Regular Days (Mon/Tue/Wed/Thu/Sat) ────────────────────────────────
    public TimeOnly RegularStartTime   { get; set; } = new TimeOnly(8, 0);
    public TimeOnly RegularEndTime     { get; set; } = new TimeOnly(13, 0);
    public int      RegularTotalPeriods { get; set; } = 6;
    public bool     RegularBreakAllowed { get; set; } = true;
    public TimeOnly RegularBreakStart  { get; set; } = new TimeOnly(10, 0);
    public TimeOnly RegularBreakEnd    { get; set; } = new TimeOnly(10, 20);
    public int      RegularGapMinutes  { get; set; } = 0;

    // ── Friday ────────────────────────────────────────────────────────────
    public TimeOnly FridayStartTime    { get; set; } = new TimeOnly(8, 0);
    public TimeOnly FridayEndTime      { get; set; } = new TimeOnly(11, 30);
    public int      FridayTotalPeriods { get; set; } = 4;
    public bool     FridayBreakAllowed { get; set; } = false;
    public TimeOnly FridayBreakStart   { get; set; } = new TimeOnly(10, 0);
    public TimeOnly FridayBreakEnd     { get; set; } = new TimeOnly(10, 10);
    public int      FridayGapMinutes   { get; set; } = 0;

    // ── Sunday ────────────────────────────────────────────────────────────
    public bool SundayEnabled { get; set; } = false;
    public TimeOnly SundayStartTime { get; set; } = new TimeOnly(8, 0);
    public TimeOnly SundayEndTime   { get; set; } = new TimeOnly(13, 0);

    // ── Working Days (JSON array of DayOfWeek ints: 1=Mon … 7=Sun) ────────
    // Default: Mon(1) Tue(2) Wed(3) Thu(4) Sat(6) are working days
    public string WorkingDaysJson { get; set; } = "[1,2,3,4,6]";

    // ── Break placement: break inserted AFTER this period number ──────────
    public int RegularBreakAfterPeriod { get; set; } = 3;
    public int FridayBreakAfterPeriod  { get; set; } = 2;

    // ── Break duration (minutes) ───────────────────────────────────────────
    public int RegularBreakDuration { get; set; } = 10;
    public int FridayBreakDuration  { get; set; } = 10;

    // ── Special / Gazette Holidays (stored as JSON array of dates) ────────
    public string HolidaysJson { get; set; } = "[]";

    // ── Validation Rules ──────────────────────────────────────────────────
    public int MinPeriodDurationMinutes { get; set; } = 20;
    public int MaxPeriodDurationMinutes { get; set; } = 60;
    public int MaxPeriodsPerDay         { get; set; } = 10;

    // ── App Branding ──────────────────────────────────────────────────────────
    public string AppName { get; set; } = "Dev_Soloutions";

    // ── Admission Numbers ─────────────────────────────────────────────────────
    // Format: {Prefix}-{Year?}-{Seq padded to AdmissionNoPadding digits},
    // e.g. "ADM-2026-0001" or "ADM-0001" when the year part is disabled.
    // A campus-specific settings row can define its own prefix so each campus
    // numbers its own admissions (e.g. "ADM-N" for North campus).
    public string AdmissionNoPrefix      { get; set; } = "ADM";
    public bool   AdmissionNoIncludeYear { get; set; } = true;
    public int    AdmissionNoPadding     { get; set; } = 4;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int?     UpdatedBy { get; set; }
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }
}
