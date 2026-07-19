namespace SchoolManagement.API.DTOs.Settings;

public class SchoolSettingsDto
{
    // Regular days
    public string RegularStartTime    { get; set; } = "08:00";
    public string RegularEndTime      { get; set; } = "13:00";
    public int    RegularTotalPeriods { get; set; } = 6;
    public bool   RegularBreakAllowed { get; set; } = true;
    public string RegularBreakStart   { get; set; } = "10:00";
    public string RegularBreakEnd     { get; set; } = "10:20";
    public int    RegularGapMinutes   { get; set; } = 0;

    // Friday
    public string FridayStartTime    { get; set; } = "08:00";
    public string FridayEndTime      { get; set; } = "11:30";
    public int    FridayTotalPeriods { get; set; } = 4;
    public bool   FridayBreakAllowed { get; set; } = false;
    public string FridayBreakStart   { get; set; } = "10:00";
    public string FridayBreakEnd     { get; set; } = "10:10";
    public int    FridayGapMinutes   { get; set; } = 0;

    // Sunday
    public bool   SundayEnabled   { get; set; } = false;
    public string SundayStartTime { get; set; } = "08:00";
    public string SundayEndTime   { get; set; } = "13:00";

    // Working days (1=Mon, 2=Tue, 3=Wed, 4=Thu, 5=Fri, 6=Sat, 7=Sun)
    public List<int> WorkingDays { get; set; } = new() { 1, 2, 3, 4, 6 };

    // Break placement
    public int RegularBreakAfterPeriod { get; set; } = 3;
    public int FridayBreakAfterPeriod  { get; set; } = 2;

    // Break duration
    public int RegularBreakDuration { get; set; } = 20;
    public int FridayBreakDuration  { get; set; } = 20;

    // Holidays
    public List<string> Holidays { get; set; } = new();

    // Validation rules
    public int MinPeriodDurationMinutes { get; set; } = 20;
    public int MaxPeriodDurationMinutes { get; set; } = 60;
    public int MaxPeriodsPerDay         { get; set; } = 10;

    // App branding
    public string AppName { get; set; } = "Dev_Soloutions";

    // Admission numbers
    public string AdmissionNoPrefix      { get; set; } = "ADM";
    public bool   AdmissionNoIncludeYear { get; set; } = true;
    public int    AdmissionNoPadding     { get; set; } = 4;
}
