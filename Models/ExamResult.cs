namespace SchoolManagement.API.Models;

/// <summary>
/// Stores a student's obtained marks for an exam paper,
/// broken down by section (optional).
/// </summary>
public class ExamResult : BaseEntity
{
    public int ExamResultId  { get; set; }
    public int ExamPaperId   { get; set; }
    public int StudentId     { get; set; }

    // ── Marks ────────────────────────────────────────────────────────────────
    /// <summary>Total obtained marks across all sections.</summary>
    public decimal ObtainedMarks    { get; set; }

    /// <summary>Section-wise marks stored as JSON, e.g. {"SectionA":15,"SectionB":22}.</summary>
    public string? SectionMarksJson { get; set; }

    // ── Computed (stored for quick reporting) ─────────────────────────────────
    public bool   IsAbsent   { get; set; } = false;
    public bool   IsPass     { get; set; } = false;

    /// <summary>Grade: A+, A, B, C, D, F following Pakistan grading scale.</summary>
    public string? Grade     { get; set; }

    /// <summary>Percentage obtained.</summary>
    public decimal? Percentage { get; set; }

    /// <summary>Position / rank in class (calculated after all results entered).</summary>
    public int?    ClassRank  { get; set; }

    // ── Remarks ───────────────────────────────────────────────────────────────
    public string? Remarks { get; set; }

    /// <summary>User (teacher/admin) who entered/verified the result.</summary>
    public int? EnteredByUserId { get; set; }
    public DateTime? EnteredAt  { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public ExamPaper ExamPaper       { get; set; } = null!;
    public Student   Student         { get; set; } = null!;
    public User?     EnteredByUser   { get; set; }
}
