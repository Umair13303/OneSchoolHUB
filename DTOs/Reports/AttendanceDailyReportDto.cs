namespace SchoolManagement.API.DTOs.Reports;

public class AttendanceDailyReportDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public DateOnly Date { get; set; }
    public int TotalStudents { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int NotMarked { get; set; }
    public List<AttendanceDailyPeriodDto> Periods { get; set; } = [];
}

public class AttendanceDailyPeriodDto
{
    public int PeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public List<AttendanceDailyRowDto> Rows { get; set; } = [];
}

public class AttendanceDailyRowDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string Status { get; set; } = "Not Marked";
    public string? Remarks { get; set; }
}
