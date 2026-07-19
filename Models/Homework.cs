namespace SchoolManagement.API.Models;

public class Homework : BaseEntity
{
    public int HomeworkId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly AssignedDate { get; set; }
    public DateOnly DueDate { get; set; }
    public int? FileId { get; set; }

    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public User Teacher { get; set; } = null!;
    public FileStore? File { get; set; }
    public ICollection<HomeworkSubmission> Submissions { get; set; } = new List<HomeworkSubmission>();
}
