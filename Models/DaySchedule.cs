namespace SchoolManagement.API.Models;

/// <summary>
/// Per-day school timing configuration. One row per day of week (1=Mon … 7=Sun).
/// </summary>
public class DaySchedule
{
    public int      Id              { get; set; }
    public int      ProfileId       { get; set; }
    public ScheduleProfile? Profile { get; set; }
    public int      DayOfWeek       { get; set; }  // 1=Mon, 2=Tue, 3=Wed, 4=Thu, 5=Fri, 6=Sat, 7=Sun
    public bool     IsWorkingDay    { get; set; } = true;
    public TimeOnly StartTime       { get; set; } = new TimeOnly(8, 0);
    public TimeOnly EndTime         { get; set; } = new TimeOnly(13, 0);
    public int      NumberOfPeriods { get; set; } = 6;
    public bool     HasBreak        { get; set; } = false;
    public int      BreakAfterPeriod{ get; set; } = 3;
    public int      BreakDuration   { get; set; } = 10;
    public DateTime UpdatedAt       { get; set; } = DateTime.UtcNow;
}
