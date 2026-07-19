namespace SchoolManagement.API.DTOs.Reports;

public class AttendanceMonthlyReportDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public int WorkingDays { get; set; }
    public List<AttendanceMonthlyRowDto> Students { get; set; } = [];
}

public class AttendanceMonthlyRowDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public double AttendancePercent =>
        (Present + Absent + Late) == 0 ? 0
        : Math.Round((Present + Late) * 100.0 / (Present + Absent + Late), 2);
}
