namespace SchoolManagement.API.DTOs.Exam;

/// <summary>View-model for printable exam paper PDF (student copy / optional answer key).</summary>
public class ExamPaperPdfModel
{
    public string SchoolName { get; set; } = "School";
    public string? CampusName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
    public int TotalMarks { get; set; }
    public int PassMarks { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Instructions { get; set; }
    public string? SyllabusNote { get; set; }
    public bool IncludeAnswers { get; set; }
    public string PrintedBy { get; set; } = "User";
    public DateTime PrintedOn { get; set; } = DateTime.Now;
    public List<ExamPaperPdfSection> Sections { get; set; } = new();
}

public class ExamPaperPdfSection
{
    public string SectionName { get; set; } = string.Empty;
    public string SectionType { get; set; } = string.Empty;
    public int AllocatedMarks { get; set; }
    public string? SectionNote { get; set; }
    public int? AttemptQuestions { get; set; }
    public List<ExamPaperPdfQuestion> Questions { get; set; } = new();
}

public class ExamPaperPdfQuestion
{
    public int Number { get; set; }
    /// <summary>1=MCQ, 2=T/F, 3=Fill, 4=Short, 5=Long</summary>
    public int QuestionTypeId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public int Marks { get; set; }
    public string? QuestionNote { get; set; }
    public string? CorrectAnswer { get; set; }
    public bool? IsTrue { get; set; }
    public List<ExamPaperPdfOption> Options { get; set; } = new();
}

public class ExamPaperPdfOption
{
    public string Label { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
