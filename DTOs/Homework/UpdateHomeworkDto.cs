namespace SchoolManagement.API.DTOs.Homework;

public class UpdateHomeworkDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly DueDate { get; set; }
    public int? FileId { get; set; }
}
