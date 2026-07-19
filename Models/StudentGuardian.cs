namespace SchoolManagement.API.Models;

public class StudentGuardian
{
    public int GuardianId { get; set; }
    public int StudentId { get; set; }
    public int? UserId { get; set; }
    public string? Relation { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Occupation { get; set; }
    public string? CNIC { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Student Student { get; set; } = null!;
    public User? User { get; set; }
}
