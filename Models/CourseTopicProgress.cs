namespace SchoolManagement.API.Models;

/// <summary>
/// Plan-scoped curriculum completion for a topic (NOT teacher-specific).
/// Teacher change must never reset these rows.
/// </summary>
public class CourseTopicProgress : BaseEntity
{
    public int CourseTopicProgressId { get; set; }
    public int CourseTopicId { get; set; }
    /// <summary>NotStarted | InProgress | Completed</summary>
    public string Status { get; set; } = "NotStarted";
    public DateTime? CompletedAt { get; set; }
    public int? CompletedByTeacherId { get; set; }
    public int? LastUpdatedByTeacherId { get; set; }

    public CourseTopic CourseTopic { get; set; } = null!;
    public User? CompletedByTeacher { get; set; }
    public User? LastUpdatedByTeacher { get; set; }
}
