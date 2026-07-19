namespace SchoolManagement.API.DTOs.Attendance;

public class AttendanceSummaryDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public int TotalDays { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public double AttendancePercent => TotalDays == 0 ? 0 : Math.Round((Present + Late) * 100.0 / TotalDays, 2);
}
