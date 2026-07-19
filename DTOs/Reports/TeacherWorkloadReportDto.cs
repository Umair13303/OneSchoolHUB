namespace SchoolManagement.API.DTOs.Reports;

public class TeacherWorkloadReportDto
{
    public int AcademicYearId { get; set; }
    public string AcademicYearLabel { get; set; } = string.Empty;
    public List<TeacherWorkloadRowDto> Teachers { get; set; } = [];
}

public class TeacherWorkloadRowDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int TimetablePeriodsPerWeek { get; set; }
    public int HomeworkAssigned { get; set; }
    public List<string> ClassesTeaching { get; set; } = [];
}
