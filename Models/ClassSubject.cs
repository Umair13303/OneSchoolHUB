namespace SchoolManagement.API.Models;

public class ClassSubject : BaseEntity
{
    public int Id { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int? TeacherId { get; set; }
    public bool IsActive { get; set; } = true;

    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public User? Teacher { get; set; }
}
