namespace SchoolManagement.API.Models;

public class Class : BaseEntity
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Homeroom / class teacher — owns the class for the full day (early-grade model).</summary>
    public int? ClassTeacherId { get; set; }
    public User? ClassTeacher { get; set; }

    public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
    public ICollection<StudentClassEnrollment> Enrollments { get; set; } = new List<StudentClassEnrollment>();
}
