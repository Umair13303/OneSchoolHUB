namespace SchoolManagement.API.DTOs.Academics;

public class AcademicCalendarEventDto
{
    public int      AcademicCalendarEventId { get; set; }
    public int      AcademicYearId          { get; set; }
    public int      CalendarEventTypeId     { get; set; }
    public string   EventTypeName           { get; set; } = string.Empty;
    public string   EventTypeColor          { get; set; } = string.Empty;
    public string   EventTypeIcon           { get; set; } = string.Empty;
    public string   Title                   { get; set; } = string.Empty;
    public DateOnly StartDate               { get; set; }
    public DateOnly? EndDate                { get; set; }
    public string?  Description             { get; set; }
}

public class CreateCalendarEventDto
{
    public int      CalendarEventTypeId { get; set; }
    public string   Title               { get; set; } = string.Empty;
    public DateOnly StartDate           { get; set; }
    public DateOnly? EndDate            { get; set; }
    public string?  Description         { get; set; }
}

public class UpdateCalendarEventDto
{
    public int      CalendarEventTypeId { get; set; }
    public string   Title               { get; set; } = string.Empty;
    public DateOnly StartDate           { get; set; }
    public DateOnly? EndDate            { get; set; }
    public string?  Description         { get; set; }
}
