namespace SchoolManagement.API.DTOs.Exam;

public class ExamPaperSectionDto
{
    public int    ExamPaperSectionId { get; set; }
    public string SectionName        { get; set; } = string.Empty;
    public string SectionType        { get; set; } = string.Empty;
    public int    AllocatedMarks     { get; set; }
    public int?   TotalQuestions     { get; set; }
    public int?   AttemptQuestions   { get; set; }
    public int?   MarksPerQuestion   { get; set; }
    public string? SectionNote       { get; set; }
    public int    SortOrder          { get; set; }
}

public class CreateExamPaperSectionDto
{
    public string  SectionName       { get; set; } = string.Empty;
    /// <summary>Objective | ShortAnswer | LongAnswer | Practical | Oral</summary>
    public string  SectionType       { get; set; } = "ShortAnswer";
    public int     AllocatedMarks    { get; set; }
    public int?    TotalQuestions    { get; set; }
    public int?    AttemptQuestions  { get; set; }
    public int?    MarksPerQuestion  { get; set; }
    public string? SectionNote       { get; set; }
    public int     SortOrder         { get; set; }
}
