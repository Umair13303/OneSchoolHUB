namespace SchoolManagement.API.DTOs.Homework;

public class CreateHomeworkDto
{
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly AssignedDate { get; set; }
    public DateOnly DueDate { get; set; }
    public int? FileId { get; set; }
}
