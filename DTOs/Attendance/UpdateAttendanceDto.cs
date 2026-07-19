namespace SchoolManagement.API.DTOs.Attendance;

public class UpdateAttendanceDto
{
    public string Status { get; set; } = string.Empty;  // Present, Absent, Late
    public string? Remarks { get; set; }
}
