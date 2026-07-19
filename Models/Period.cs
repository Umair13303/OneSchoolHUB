namespace SchoolManagement.API.Models;

public class Period
{
    public int PeriodId { get; set; }
    public int PeriodNo { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsBreak { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }
}
