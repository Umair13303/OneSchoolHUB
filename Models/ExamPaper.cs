namespace SchoolManagement.API.Models;

/// <summary>
/// Exam types following Pakistan's educational pattern (Class 1–12).
/// </summary>
public enum ExamType
{
    Quiz        = 1,   // Short chapter/topic quiz — all classes
    MonthlyTest = 2,   // Monthly assessment — all classes
    MidTerm     = 3,   // Mid-semester / half-yearly — all classes
    PreBoard    = 4,   // Practice board exam — Class 9–12 only
    FinalExam   = 5    // Annual final examination — all classes
}

/// <summary>
/// Education level grouping that determines the paper pattern.
/// Primary (1–5): mostly written/oral, simple subjective.
/// Middle (6–8): objective + short Q + long Q.
/// Matric (9–10): MCQs + short Q + long Q/comprehensive (board pattern).
/// Inter (11–12): same as matric, more depth.
/// </summary>
public enum ClassGroup
{
    Primary = 1,   // Class 1–5
    Middle  = 2,   // Class 6–8
    Matric  = 3,   // Class 9–10  (SSC Part-I & II)
    Inter   = 4    // Class 11–12 (HSSC Part-I & II)
}

/// <summary>
/// An exam paper created by a teacher or admin for a specific class + subject.
/// Covers all exam types in the Pakistan (Punjab/Federal/Sindh/KPK) education system.
/// </summary>
public class ExamPaper : BaseEntity
{
    public int ExamPaperId { get; set; }

    // ── Core identifiers ─────────────────────────────────────────────────────
    public int         AcademicYearId { get; set; }
    public int         ClassId        { get; set; }
    public int         SubjectId      { get; set; }
    /// <summary>User (teacher/admin) who created the paper.</summary>
    public int         CreatedByUserId { get; set; }

    // ── Exam classification ───────────────────────────────────────────────────
    public ExamType    ExamType   { get; set; }
    public ClassGroup  ClassGroup { get; set; }

    /// <summary>
    /// Display name e.g. "1st Monthly Test – Mathematics – Class 8A".
    /// Auto-generated if left blank.
    /// </summary>
    public string      Title      { get; set; } = string.Empty;

    // ── Marks ────────────────────────────────────────────────────────────────
    public int  TotalMarks { get; set; }
    public int  PassMarks  { get; set; }

    // ── Duration & instruction ────────────────────────────────────────────────
    /// <summary>Duration of the exam in minutes.</summary>
    public int?    DurationMinutes { get; set; }
    public string? Instructions    { get; set; }
    public string? SyllabusNote    { get; set; }   // e.g. "Ch 1 to Ch 5"

    // ── State ────────────────────────────────────────────────────────────────
    public bool IsDraft    { get; set; } = true;
    public bool IsLocked   { get; set; } = false;  // locked once results entered

    // ── Navigation ───────────────────────────────────────────────────────────
    public AcademicYear AcademicYear   { get; set; } = null!;
    public Class        Class          { get; set; } = null!;
    public Subject      Subject        { get; set; } = null!;
    public User         CreatedByUser  { get; set; } = null!;

    public ICollection<ExamPaperSection> Sections  { get; set; } = new List<ExamPaperSection>();
    public ICollection<ExamSchedule>     Schedules { get; set; } = new List<ExamSchedule>();
    public ICollection<ExamResult>       Results   { get; set; } = new List<ExamResult>();
    public ICollection<ExamQuestion>     Questions { get; set; } = new List<ExamQuestion>();
}
