namespace SchoolManagement.API.Models;

public class Subject : BaseEntity
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();
}
