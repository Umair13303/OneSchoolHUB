namespace SchoolManagement.API.Models;

/// <summary>
/// Append-only individual student performance against a course topic
/// (e.g. oral recitation / sabaq). Never overwrite — insert a new row.
/// </summary>
public class StudentTopicPerformance : BaseEntity
{
    public int StudentTopicPerformanceId { get; set; }
    public int StudentId { get; set; }
    public int CourseTopicId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int AcademicYearId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly PerformanceDate { get; set; }
    /// <summary>OralRecitation | Participation | Other</summary>
    public string PerformanceType { get; set; } = "OralRecitation";
    /// <summary>AssessmentResultLookup.Code</summary>
    public string ResultStatus { get; set; } = string.Empty;
    public string? Remarks { get; set; }

    public Student Student { get; set; } = null!;
    public CourseTopic CourseTopic { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
    public User Teacher { get; set; } = null!;
}
