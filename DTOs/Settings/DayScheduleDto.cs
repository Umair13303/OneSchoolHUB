namespace SchoolManagement.API.DTOs.Settings;

public class DayScheduleDto
{
    public int    DayOfWeek        { get; set; }
    public string DayName          { get; set; } = "";
    public bool   IsWorkingDay     { get; set; }
    public string StartTime        { get; set; } = "08:00";
    public string EndTime          { get; set; } = "13:00";
    public int    NumberOfPeriods  { get; set; } = 6;
    public bool   HasBreak         { get; set; }
    public int    BreakAfterPeriod { get; set; } = 3;
    public int    BreakDuration    { get; set; } = 10;
}

public class UpdateDayScheduleDto
{
    public bool   IsWorkingDay     { get; set; }
    public string StartTime        { get; set; } = "08:00";
    public string EndTime          { get; set; } = "13:00";
    public int    NumberOfPeriods  { get; set; } = 6;
    public bool   HasBreak         { get; set; }
    public int    BreakAfterPeriod { get; set; } = 3;
    public int    BreakDuration    { get; set; } = 10;
}

public class BulkUpdateDayScheduleDto
{
    public List<UpdateDayScheduleRequest> Days { get; set; } = new();
}

public class UpdateDayScheduleRequest : UpdateDayScheduleDto
{
    public int DayOfWeek { get; set; }
}
