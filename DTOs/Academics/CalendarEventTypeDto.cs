namespace SchoolManagement.API.DTOs.Academics;

public class CalendarEventTypeDto
{
    public int    CalendarEventTypeId { get; set; }
    public string Name                { get; set; } = string.Empty;
    public string Color               { get; set; } = string.Empty;
    public string Icon                { get; set; } = string.Empty;
    public int    SortOrder           { get; set; }
}

public class CreateCalendarEventTypeDto
{
    public string Name      { get; set; } = string.Empty;
    public string Color     { get; set; } = "#6b7280";
    public string Icon      { get; set; } = "event";
    public int    SortOrder { get; set; } = 0;
}
