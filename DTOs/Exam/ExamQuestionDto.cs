using SchoolManagement.API.Models;

namespace SchoolManagement.API.DTOs.Exam;

public class ExamQuestionOptionDto
{
    public int    ExamQuestionOptionId { get; set; }
    public string OptionLabel          { get; set; } = string.Empty;
    public string OptionText           { get; set; } = string.Empty;
    public bool   IsCorrect            { get; set; }
    public int    SortOrder            { get; set; }
}

public class ExamQuestionDto
{
    public int              ExamQuestionId      { get; set; }
    public int              ExamPaperId         { get; set; }
    public int?             ExamPaperSectionId  { get; set; }
    public string?          SectionName         { get; set; }
    public string           QuestionType        { get; set; } = string.Empty;   // enum label
    public int              QuestionTypeId      { get; set; }
    public string           QuestionText        { get; set; } = string.Empty;
    public string           Language            { get; set; } = "en";
    public int              Marks               { get; set; }
    public int              SortOrder           { get; set; }
    public string?          CorrectAnswer       { get; set; }
    public bool?            IsTrue              { get; set; }
    public string?          QuestionNote        { get; set; }
    public List<ExamQuestionOptionDto> Options  { get; set; } = new();
}

public class CreateExamQuestionOptionDto
{
    public string OptionLabel { get; set; } = string.Empty;
    public string OptionText  { get; set; } = string.Empty;
    public bool   IsCorrect   { get; set; } = false;
    public int    SortOrder   { get; set; }
}

public class CreateExamQuestionDto
{
    public int         ExamPaperId        { get; set; }
    public int?        ExamPaperSectionId { get; set; }
    /// <summary>1=MultipleChoice,2=TrueFalse,3=FillInBlanks,4=ShortQuestion,5=LongQuestion</summary>
    public QuestionType QuestionType      { get; set; }
    public string      QuestionText       { get; set; } = string.Empty;
    /// <summary>en | ur | math</summary>
    public string      Language           { get; set; } = "en";
    public int         Marks              { get; set; } = 1;
    public int         SortOrder          { get; set; }
    public string?     CorrectAnswer      { get; set; }
    public bool?       IsTrue             { get; set; }
    public string?     QuestionNote       { get; set; }

    /// <summary>For MultipleChoice — must supply exactly 4 options with one IsCorrect=true.</summary>
    public List<CreateExamQuestionOptionDto> Options { get; set; } = new();
}

public class UpdateExamQuestionDto
{
    public int?        ExamPaperSectionId { get; set; }
    public string?     QuestionText       { get; set; }
    public string?     Language           { get; set; }
    public int?        Marks              { get; set; }
    public int?        SortOrder          { get; set; }
    public string?     CorrectAnswer      { get; set; }
    public bool?       IsTrue             { get; set; }
    public string?     QuestionNote       { get; set; }
    public List<CreateExamQuestionOptionDto>? Options { get; set; }
}

/// <summary>Bulk-save all questions for a paper in one call (replaces existing questions).</summary>
public class SavePaperQuestionsDto
{
    public int ExamPaperId { get; set; }
    public List<CreateExamQuestionDto> Questions { get; set; } = new();
}
