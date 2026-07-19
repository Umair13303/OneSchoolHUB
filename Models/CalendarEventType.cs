namespace SchoolManagement.API.Models;

public class CalendarEventType : BaseEntity
{
    public int    CalendarEventTypeId { get; set; }
    public string Name                { get; set; } = string.Empty;
    public string Color               { get; set; } = "#6b7280"; // hex color
    public string Icon                { get; set; } = "event";   // material icon name
    public int    SortOrder           { get; set; } = 0;

    public ICollection<AcademicCalendarEvent> Events { get; set; } = new List<AcademicCalendarEvent>();
}
