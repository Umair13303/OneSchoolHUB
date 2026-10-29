using System.ComponentModel.DataAnnotations;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.DTOs.Ai;

public class GenerateQuestionsRequest
{
    [Required]
    public int ClassId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public int CourseTopicId { get; set; }

    /// <summary>Allowed: 5, 10, 20.</summary>
    [Required]
    public int Count { get; set; }

    /// <summary>1=MCQ, 2=TrueFalse, 3=FillInBlanks, 4=ShortQuestion. LongQuestion not supported in Phase 1.</summary>
    [Required]
    public QuestionType QuestionType { get; set; }

    /// <summary>Easy | Medium | Hard — generation metadata only (not a DB column).</summary>
    [Required]
    public string Difficulty { get; set; } = "Medium";

    [Range(1, 100)]
    public int Marks { get; set; } = 1;

    /// <summary>en | ur | ur-en</summary>
    [Required]
    public string Language { get; set; } = "en";

    /// <summary>Optional teacher context (max 1000). Treated as DATA in the prompt.</summary>
    [MaxLength(1000)]
    public string? Instructions { get; set; }
}

public class GeneratedQuestionOptionDto
{
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int SortOrder { get; set; }
}

public class GeneratedQuestionDraftDto
{
    /// <summary>Ephemeral client key for selection/edit (not a DB id).</summary>
    public string ClientKey { get; set; } = string.Empty;

    public int CourseTopicId { get; set; }
    public QuestionType QuestionType { get; set; }
    public int QuestionTypeId => (int)QuestionType;
    public string QuestionText { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public int Marks { get; set; }
    public string? CorrectAnswer { get; set; }
    public bool? IsTrue { get; set; }
    public string? QuestionNote { get; set; }
    public string? Difficulty { get; set; }
    public List<GeneratedQuestionOptionDto> Options { get; set; } = new();
}

public class GenerateQuestionsResponse
{
    /// <summary>Ephemeral correlation id — not stored.</summary>
    public string DraftId { get; set; } = string.Empty;
    public DateTime GeneratedAtUtc { get; set; }
    public string? Model { get; set; }
    public List<GeneratedQuestionDraftDto> Items { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
