namespace SchoolManagement.API.Models;

public class AcademicCalendarEvent : BaseEntity
{
    public int    AcademicCalendarEventId { get; set; }
    public int    AcademicYearId          { get; set; }
    public int    CalendarEventTypeId     { get; set; }
    public string Title                   { get; set; } = string.Empty;
    public DateOnly StartDate             { get; set; }
    public DateOnly? EndDate              { get; set; }
    public string? Description            { get; set; }

    public AcademicYear        AcademicYear       { get; set; } = null!;
    public CalendarEventType   CalendarEventType  { get; set; } = null!;
}
