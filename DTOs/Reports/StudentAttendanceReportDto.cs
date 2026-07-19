namespace SchoolManagement.API.DTOs.Reports;

public class StudentAttendanceReportDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public int TotalRecords { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public double AttendancePercent =>
        TotalRecords == 0 ? 0 : Math.Round((Present + Late) * 100.0 / TotalRecords, 2);
    public List<StudentAttendanceDayDto> Days { get; set; } = [];
}

public class StudentAttendanceDayDto
{
    public DateOnly Date { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}
