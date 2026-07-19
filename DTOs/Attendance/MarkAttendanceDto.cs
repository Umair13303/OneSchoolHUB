namespace SchoolManagement.API.DTOs.Attendance;

public class MarkAttendanceDto
{
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public int PeriodId { get; set; }
    public DateOnly Date { get; set; }
    public string Status { get; set; } = string.Empty;  // Present, Absent, Late
    public string? Remarks { get; set; }
}
