namespace SchoolManagement.API.Models;

public class Student : BaseEntity
{
    public int StudentId { get; set; }
    public string AdmissionNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? BloodGroup { get; set; }
    public string? Religion { get; set; }
    public string? Nationality { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public int? PhotoFileId { get; set; }
    public DateOnly AdmissionDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    public bool IsActive { get; set; } = true;
    public DateOnly? LeavingDate   { get; set; }
    public string?   LeavingReason { get; set; }

    public FileStore? Photo { get; set; }
    public ICollection<StudentGuardian> Guardians { get; set; } = new List<StudentGuardian>();
    public ICollection<StudentClassEnrollment> Enrollments { get; set; } = new List<StudentClassEnrollment>();
}
