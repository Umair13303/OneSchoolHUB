namespace SchoolManagement.API.DTOs.Settings;

public class ScheduleProfileDto
{
    public int                  Id        { get; set; }
    public string               Name      { get; set; } = "";
    public bool                 IsActive  { get; set; }
    public DateTime             CreatedAt { get; set; }
    public List<DayScheduleDto> Days      { get; set; } = new();
}

public class CreateScheduleProfileDto
{
    public string Name { get; set; } = "";
}

public class RenameScheduleProfileDto
{
    public string Name { get; set; } = "";
}
