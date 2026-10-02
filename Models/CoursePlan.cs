namespace SchoolManagement.API.Models;

/// <summary>
/// Academic-year-scoped curriculum plan for one Class + Subject.
/// Only one non-archived plan per AcademicYear + Class + Subject + tenant.
/// </summary>
public class CoursePlan : BaseEntity
{
    public int CoursePlanId { get; set; }
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    /// <summary>Draft | Published | Archived</summary>
    public string Status { get; set; } = "Draft";
    public DateTime? PublishedAt { get; set; }

    public AcademicYear AcademicYear { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public ICollection<CourseChapter> Chapters { get; set; } = new List<CourseChapter>();
}
