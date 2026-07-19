namespace SchoolManagement.API.Models;

public class ScheduleProfile
{
    public int      Id        { get; set; }
    public string   Name      { get; set; } = "";
    public bool     IsActive  { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }

    public ICollection<DaySchedule>   Days    { get; set; } = new List<DaySchedule>();
    public ICollection<ProfilePeriod> Periods { get; set; } = new List<ProfilePeriod>();
}
