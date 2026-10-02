namespace SchoolManagement.API.Models;

/// <summary>
/// Lightweight classroom quiz / class test / oral test session.
/// Separate from formal ExamPaper.
/// </summary>
public class ClassAssessment : BaseEntity
{
    public int ClassAssessmentId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int AcademicYearId { get; set; }
    public int? CourseTopicId { get; set; }
    public int? CourseTeachingLogId { get; set; }
    public int TeacherId { get; set; }
    public DateOnly AssessmentDate { get; set; }
    public string Title { get; set; } = string.Empty;
    /// <summary>Quiz | ClassTest | OralTest | Other</summary>
    public string AssessmentType { get; set; } = "Quiz";
    public int? TotalMarks { get; set; }
    public int? TotalQuestions { get; set; }
    public string? Notes { get; set; }

    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
    public CourseTopic? CourseTopic { get; set; }
    public CourseTeachingLog? CourseTeachingLog { get; set; }
    public User Teacher { get; set; } = null!;
    public ICollection<ClassAssessmentResult> Results { get; set; } = new List<ClassAssessmentResult>();
}

/// <summary>Per-student result for a ClassAssessment session.</summary>
public class ClassAssessmentResult : BaseEntity
{
    public int ClassAssessmentResultId { get; set; }
    public int ClassAssessmentId { get; set; }
    public int StudentId { get; set; }
    public decimal? ObtainedMarks { get; set; }
    /// <summary>Optional lookup code (AssessmentResultLookup.Code).</summary>
    public string? Status { get; set; }
    public string? Remarks { get; set; }

    public ClassAssessment ClassAssessment { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
