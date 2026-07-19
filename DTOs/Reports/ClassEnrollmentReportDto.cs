namespace SchoolManagement.API.DTOs.Reports;

public class ClassEnrollmentReportDto
{
    public int AcademicYearId { get; set; }
    public string AcademicYearLabel { get; set; } = string.Empty;
    public int TotalClasses { get; set; }
    public int TotalStudents { get; set; }
    public List<ClassEnrollmentRowDto> Classes { get; set; } = [];
}

public class ClassEnrollmentRowDto
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public int TotalStudents { get; set; }
    public int Active { get; set; }
    public int Withdrawn { get; set; }
    public int Promoted { get; set; }
    public List<EnrolledStudentDto> Students { get; set; } = [];
}

public class EnrolledStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public DateOnly AdmissionDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
