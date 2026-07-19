namespace SchoolManagement.API.DTOs.Settings;

public class ProfilePeriodDto
{
    public int    DayOfWeek       { get; set; }
    public string DayName         { get; set; } = "";
    public int    PeriodNo        { get; set; }
    public string PeriodName      { get; set; } = "";
    public string StartTime       { get; set; } = "";
    public string EndTime         { get; set; } = "";
    public int    DurationMinutes { get; set; }
    public bool   IsBreak         { get; set; }
    public int    SortOrder       { get; set; }
    public int    PeriodId        { get; set; }  // DB Periods.PeriodId FK for timetable entries
}

public class SaveProfilePeriodsDto
{
    public List<ProfilePeriodDto> Periods { get; set; } = new();
}
