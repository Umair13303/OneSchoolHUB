namespace SchoolManagement.API.DTOs.Homework;

public class SubmissionDto
{
    public int SubmissionId { get; set; }
    public int HomeworkId { get; set; }
    public string HomeworkTitle { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public DateTime? SubmittedAt { get; set; }
    public int? FileId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
