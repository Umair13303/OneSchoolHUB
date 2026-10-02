namespace SchoolManagement.API.Models;

public class CourseChapter : BaseEntity
{
    public int CourseChapterId { get; set; }
    public int CoursePlanId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SortOrder { get; set; }

    public CoursePlan CoursePlan { get; set; } = null!;
    public ICollection<CourseTopic> Topics { get; set; } = new List<CourseTopic>();
    public ICollection<CourseMaterial> Materials { get; set; } = new List<CourseMaterial>();
}
