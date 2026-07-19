namespace SchoolManagement.API.DTOs.Timetable;

public class PeriodDto
{
    public int      PeriodId   { get; set; }
    public int      PeriodNo   { get; set; }
    public string   PeriodName { get; set; } = string.Empty;
    public TimeOnly StartTime  { get; set; }
    public TimeOnly EndTime    { get; set; }
    public bool     IsBreak    { get; set; }
    public bool     IsActive   { get; set; }
}
