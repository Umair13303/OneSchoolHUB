namespace SchoolManagement.API.Models;

public class ProfilePeriod
{
    public int      Id              { get; set; }
    public int      ProfileId       { get; set; }
    public ScheduleProfile? Profile { get; set; }
    public int      DayOfWeek       { get; set; }  // 1=Mon … 7=Sun
    public int      PeriodNo        { get; set; }  // 0 = break
    public string   PeriodName      { get; set; } = "";
    public TimeOnly StartTime       { get; set; }
    public TimeOnly EndTime         { get; set; }
    public int      DurationMinutes { get; set; }
    public bool     IsBreak         { get; set; }
    public int      SortOrder       { get; set; }
}
