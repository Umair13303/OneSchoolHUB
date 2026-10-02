namespace SchoolManagement.API.Models;

/// <summary>
/// Daily teaching / lesson delivery — append-only activity log.
/// Separate from curriculum completion (CourseTopicProgress).
/// </summary>
public class CourseTeachingLog : BaseEntity
{
    public int CourseTeachingLogId { get; set; }
    public DateOnly TeachingDate { get; set; }
    public int TeacherId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int AcademicYearId { get; set; }
    public int CourseTopicId { get; set; }
    /// <summary>NewTopic | Revision | Practice | Assessment</summary>
    public string TeachingType { get; set; } = "NewTopic";
    public string? Remarks { get; set; }
    public string? ExtraNotes { get; set; }
    /// <summary>Optional homework created/linked from this teaching session.</summary>
    public int? HomeworkId { get; set; }

    public User Teacher { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
    public CourseTopic CourseTopic { get; set; } = null!;
    public Homework? Homework { get; set; }
}
