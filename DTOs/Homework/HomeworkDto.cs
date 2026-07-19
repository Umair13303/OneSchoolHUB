namespace SchoolManagement.API.DTOs.Homework;

public class HomeworkDto
{
    public int HomeworkId { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? Section { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly AssignedDate { get; set; }
    public DateOnly DueDate { get; set; }
    public int? FileId { get; set; }
    public int SubmissionCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
