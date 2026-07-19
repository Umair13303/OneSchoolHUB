namespace SchoolManagement.API.Models;

/// <summary>
/// A section inside an exam paper.
///
/// Pakistan paper structure by class group:
///
/// PRIMARY (1–5)
///   Section A – Written / Oral  (100% of marks)
///
/// MIDDLE (6–8)
///   Section A – Objective       (MCQs, Fill-in-blanks, T/F)
///   Section B – Short Questions
///   Section C – Long Questions
///
/// MATRIC / INTER (9–12)  [Board pattern]
///   Section A – Objective  (MCQs, typically 12–20 marks, 20 min)
///   Section B – Short Questions (attempt any X out of Y, 2–4 marks each)
///   Section C – Long / Comprehensive Questions (attempt any X out of Y, 8–12 marks each)
/// </summary>
public class ExamPaperSection : BaseEntity
{
    public int ExamPaperSectionId { get; set; }
    public int ExamPaperId        { get; set; }

    // ── Section identity ──────────────────────────────────────────────────────
    /// <summary>e.g. "Section A", "Section B", "Section C"</summary>
    public string SectionName  { get; set; } = string.Empty;

    /// <summary>
    /// "Objective"   – MCQs / T-F / Fill-in-blanks
    /// "ShortAnswer" – Short questions
    /// "LongAnswer"  – Detailed / essay / comprehensive questions
    /// "Practical"   – Lab / oral / viva (used for science practicals)
    /// "Oral"        – Oral exam (primary classes, reading/recitation)
    /// </summary>
    public string SectionType  { get; set; } = "ShortAnswer";

    /// <summary>Marks allocated to this section.</summary>
    public int    AllocatedMarks  { get; set; }

    /// <summary>Total questions printed in the paper for this section.</summary>
    public int?   TotalQuestions  { get; set; }

    /// <summary>Questions the student must attempt (optional – board exams use this).</summary>
    public int?   AttemptQuestions { get; set; }

    /// <summary>Marks per question (informational).</summary>
    public int?   MarksPerQuestion { get; set; }

    /// <summary>Additional instructions for this section, e.g. "Attempt any 5".</summary>
    public string? SectionNote { get; set; }

    public int SortOrder { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public ExamPaper ExamPaper { get; set; } = null!;
    public ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();
}
