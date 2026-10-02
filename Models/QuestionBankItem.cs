namespace SchoolManagement.API.Models;

/// <summary>
/// Reusable question bank item tagged to a CourseTopic.
/// When added to an ExamPaper, content is copied into ExamQuestion (paper remains SoT).
/// </summary>
public class QuestionBankItem : BaseEntity
{
    public int QuestionBankItemId { get; set; }
    public int CourseTopicId { get; set; }
    public QuestionType QuestionType { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public int Marks { get; set; } = 1;
    public string? CorrectAnswer { get; set; }
    public bool? IsTrue { get; set; }
    public string? QuestionNote { get; set; }
    public bool IsActive { get; set; } = true;

    public CourseTopic CourseTopic { get; set; } = null!;
    public ICollection<QuestionBankOption> Options { get; set; } = new List<QuestionBankOption>();
}

public class QuestionBankOption : BaseEntity
{
    public int QuestionBankOptionId { get; set; }
    public int QuestionBankItemId { get; set; }
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }

    public QuestionBankItem QuestionBankItem { get; set; } = null!;
}
