namespace SchoolManagement.API.Models;

/// <summary>
/// Question types for exam papers — covers Pakistan board exam patterns.
/// Supports both LTR (English, Maths) and RTL (Urdu, Arabic) text.
/// </summary>
public enum QuestionType
{
    MultipleChoice = 1,   // MCQ — choose one from 4 options (Section A)
    TrueFalse      = 2,   // Circle True/False or T/F
    FillInBlanks   = 3,   // Blank line in the sentence
    ShortQuestion  = 4,   // 2–4 marks short answer (Section B)
    LongQuestion   = 5    // 8–12 marks essay/comprehensive (Section C)
}

/// <summary>
/// A single question belonging to an exam paper (and optionally a section).
/// Supports multilingual text (Urdu RTL + Maths LaTeX notation).
/// </summary>
public class ExamQuestion : BaseEntity
{
    public int ExamQuestionId   { get; set; }
    public int ExamPaperId      { get; set; }
    /// <summary>Optional — links question to a specific section.</summary>
    public int? ExamPaperSectionId { get; set; }

    public QuestionType QuestionType { get; set; }

    /// <summary>
    /// Question text. For Urdu, store Unicode Urdu text (system renders RTL via CSS dir="rtl").
    /// For Maths, store LaTeX between $...$ delimiters e.g. "$x^2 + y^2 = z^2$".
    /// </summary>
    public string QuestionText { get; set; } = string.Empty;

    /// <summary>
    /// Language / script direction. "ur" = Urdu (RTL), "en" = English (LTR), "math" = mathematical.
    /// UI uses this to apply correct direction and font rendering.
    /// </summary>
    public string Language { get; set; } = "en";   // en | ur | math

    /// <summary>Marks allocated to this individual question.</summary>
    public int Marks { get; set; } = 1;

    /// <summary>Display order within the paper / section.</summary>
    public int SortOrder { get; set; }

    // ── Fill-in-Blanks ────────────────────────────────────────────────────────
    /// <summary>
    /// For FillInBlanks: the expected answer (shown on answer key / teacher copy).
    /// For ShortQuestion / LongQuestion: model answer / marking guide (optional).
    /// </summary>
    public string? CorrectAnswer { get; set; }

    // ── True/False ────────────────────────────────────────────────────────────
    /// <summary>For TrueFalse only: true = correct answer is True, false = False.</summary>
    public bool? IsTrue { get; set; }

    // ── Optional sub-instruction ──────────────────────────────────────────────
    /// <summary>e.g. "Attempt any one" on long questions.</summary>
    public string? QuestionNote { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public ExamPaper        ExamPaper       { get; set; } = null!;
    public ExamPaperSection? ExamPaperSection { get; set; }

    /// <summary>MCQ options — populated for MultipleChoice type.</summary>
    public ICollection<ExamQuestionOption> Options { get; set; } = new List<ExamQuestionOption>();
}

/// <summary>
/// An option for a Multiple-Choice question (A, B, C, D).
/// </summary>
public class ExamQuestionOption : BaseEntity
{
    public int ExamQuestionOptionId { get; set; }
    public int ExamQuestionId       { get; set; }

    /// <summary>e.g. "A", "B", "C", "D".</summary>
    public string OptionLabel { get; set; } = string.Empty;

    /// <summary>Option text — supports Urdu Unicode and Maths LaTeX notation.</summary>
    public string OptionText  { get; set; } = string.Empty;

    /// <summary>True if this is the correct option.</summary>
    public bool IsCorrect { get; set; } = false;

    public int SortOrder { get; set; }

    // ── Navigation ────────────────────────────────────────────────────────────
    public ExamQuestion ExamQuestion { get; set; } = null!;
}
