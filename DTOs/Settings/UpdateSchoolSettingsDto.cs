using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Settings;

public class UpdateSchoolSettingsDto
{
    // Regular days
    [Required] public string RegularStartTime    { get; set; } = "08:00";
    [Required] public string RegularEndTime      { get; set; } = "13:00";
    [Range(1, 20)] public int RegularTotalPeriods { get; set; } = 6;
    public bool   RegularBreakAllowed { get; set; } = true;
    public string RegularBreakStart   { get; set; } = "10:00";
    public string RegularBreakEnd     { get; set; } = "10:20";
    [Range(0, 30)] public int RegularGapMinutes { get; set; } = 0;

    // Friday
    [Required] public string FridayStartTime    { get; set; } = "08:00";
    [Required] public string FridayEndTime      { get; set; } = "11:30";
    [Range(1, 20)] public int FridayTotalPeriods { get; set; } = 4;
    public bool   FridayBreakAllowed { get; set; } = false;
    public string FridayBreakStart   { get; set; } = "10:00";
    public string FridayBreakEnd     { get; set; } = "10:10";
    [Range(0, 30)] public int FridayGapMinutes { get; set; } = 0;

    // Sunday
    public bool   SundayEnabled   { get; set; } = false;
    public string SundayStartTime { get; set; } = "08:00";
    public string SundayEndTime   { get; set; } = "13:00";

    // Working days (1=Mon … 7=Sun)
    public List<int> WorkingDays { get; set; } = new() { 1, 2, 3, 4, 6 };

    // Break placement: break after which period number
    [Range(0, 20)] public int RegularBreakAfterPeriod { get; set; } = 3;
    [Range(0, 20)] public int FridayBreakAfterPeriod  { get; set; } = 2;

    // Break duration (minimum 10 minutes)
    [Range(10, 60)] public int RegularBreakDuration { get; set; } = 20;
    [Range(10, 60)] public int FridayBreakDuration  { get; set; } = 20;

    // Holidays (ISO date strings yyyy-MM-dd)
    public List<string> Holidays { get; set; } = new();

    // Validation rules
    [Range(10, 30)] public int MinPeriodDurationMinutes { get; set; } = 20;
    [Range(30, 120)] public int MaxPeriodDurationMinutes { get; set; } = 60;
    [Range(1, 20)] public int MaxPeriodsPerDay           { get; set; } = 10;

    // App branding
    [MaxLength(100)] public string AppName { get; set; } = "Dev_Soloutions";

    // Admission numbers
    [MaxLength(10)] public string AdmissionNoPrefix { get; set; } = "ADM";
    public bool AdmissionNoIncludeYear { get; set; } = true;
    [Range(2, 8)] public int AdmissionNoPadding { get; set; } = 4;
}
