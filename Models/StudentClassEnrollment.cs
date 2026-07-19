namespace SchoolManagement.API.Models;

public class StudentClassEnrollment
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int ClassId { get; set; }
    public int AcademicYearId { get; set; }
    public DateOnly EnrollmentDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public string Status { get; set; } = "Active";   // Active, Promoted, Withdrawn
    public bool IsDeleted { get; set; } = false;
    public int? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }

    public Student Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
}
