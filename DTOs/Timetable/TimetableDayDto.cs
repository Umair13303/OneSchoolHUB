namespace SchoolManagement.API.DTOs.Timetable;

/// <summary>
/// One day's worth of timetable entries — handy for rendering a single-day
/// view or grouping a weekly grid by day.
/// </summary>
public class TimetableDayDto
{
    public int    DayOfWeek { get; set; }
    public string DayName   { get; set; } = string.Empty;
    public List<TimetableEntryDto> Entries { get; set; } = new();
}
