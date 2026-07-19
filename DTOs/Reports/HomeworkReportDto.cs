namespace SchoolManagement.API.DTOs.Reports;

public class HomeworkReportDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public int TotalHomework { get; set; }
    public int TotalStudents { get; set; }
    public List<HomeworkReportRowDto> Homework { get; set; } = [];
}

public class HomeworkReportRowDto
{
    public int HomeworkId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public DateOnly AssignedDate { get; set; }
    public DateOnly DueDate { get; set; }
    public int TotalStudents { get; set; }
    public int Submitted { get; set; }
    public int Reviewed { get; set; }
    public int Pending { get; set; }
    public double SubmissionPercent =>
        TotalStudents == 0 ? 0 : Math.Round(Submitted * 100.0 / TotalStudents, 2);
}
