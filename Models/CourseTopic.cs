namespace SchoolManagement.API.Models;

public class CourseTopic : BaseEntity
{
    public int CourseTopicId { get; set; }
    public int CourseChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }

    public CourseChapter CourseChapter { get; set; } = null!;
    public CourseTopicProgress? Progress { get; set; }
    public ICollection<CourseMaterial> Materials { get; set; } = new List<CourseMaterial>();
    public ICollection<CourseTopicActivity> Activities { get; set; } = new List<CourseTopicActivity>();
    public ICollection<CourseTeachingLog> TeachingLogs { get; set; } = new List<CourseTeachingLog>();
}
