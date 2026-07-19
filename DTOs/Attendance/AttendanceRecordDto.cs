namespace SchoolManagement.API.DTOs.Attendance;

public class AttendanceRecordDto
{
    public int AttendanceId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public int PeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public int MarkedBy { get; set; }
    public string MarkedByName { get; set; } = string.Empty;
    public DateTime MarkedAt { get; set; }
}
