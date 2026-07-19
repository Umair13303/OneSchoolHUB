namespace SchoolManagement.API.Models;

public class HomeworkSubmission
{
    public int SubmissionId { get; set; }
    public int HomeworkId { get; set; }
    public int StudentId { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int? FileId { get; set; }
    public string Status { get; set; } = "Pending";   // Pending, Submitted, Reviewed
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Homework Homework { get; set; } = null!;
    public Student Student { get; set; } = null!;
    public FileStore? File { get; set; }
}
