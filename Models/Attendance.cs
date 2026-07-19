namespace SchoolManagement.API.Models;

public class Attendance
{
    public int AttendanceId { get; set; }
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public int PeriodId { get; set; }
    public DateOnly Date { get; set; }
    public string Status { get; set; } = string.Empty;   // Present, Absent, Late
    public int MarkedBy { get; set; }
    public DateTime MarkedAt { get; set; } = DateTime.UtcNow;
    public string? Remarks { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }

    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public Period Period { get; set; } = null!;
    public User MarkedByUser { get; set; } = null!;
}
