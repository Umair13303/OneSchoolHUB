using SchoolManagement.API.Models;

namespace SchoolManagement.API.DTOs.Exam;

public class ExamPaperDto
{
    public int       ExamPaperId      { get; set; }
    public int       AcademicYearId   { get; set; }
    public string    AcademicYearName { get; set; } = string.Empty;
    public int       ClassId          { get; set; }
    public string    ClassName        { get; set; } = string.Empty;
    public int       SubjectId        { get; set; }
    public string    SubjectName      { get; set; } = string.Empty;
    public string    ExamType         { get; set; } = string.Empty;   // enum label
    public int       ExamTypeId       { get; set; }                   // enum int value
    public string    ClassGroup       { get; set; } = string.Empty;   // enum label
    public int       ClassGroupId     { get; set; }                   // enum int value
    public string    Title            { get; set; } = string.Empty;
    public int       TotalMarks       { get; set; }
    public int       PassMarks        { get; set; }
    public int?      DurationMinutes  { get; set; }
    public string?   Instructions     { get; set; }
    public string?   SyllabusNote     { get; set; }
    public bool      IsDraft          { get; set; }
    public bool      IsLocked         { get; set; }
    public string    CreatedByName    { get; set; } = string.Empty;
    public DateTime  CreatedAt        { get; set; }

    public List<ExamPaperSectionDto>  Sections  { get; set; } = new();
    public List<ExamScheduleDto>      Schedules { get; set; } = new();
}

public class ExamPaperListDto
{
    public int     ExamPaperId    { get; set; }
    public string  Title          { get; set; } = string.Empty;
    public string  ExamType       { get; set; } = string.Empty;
    public string  ClassName      { get; set; } = string.Empty;
    public string  SubjectName    { get; set; } = string.Empty;
    public int     TotalMarks     { get; set; }
    public int     PassMarks      { get; set; }
    public bool    IsDraft        { get; set; }
    public bool    IsLocked       { get; set; }
    public string  AcademicYear   { get; set; } = string.Empty;
    public DateOnly? ScheduledDate { get; set; }   // earliest schedule date
}

public class CreateExamPaperDto
{
    public int       AcademicYearId  { get; set; }
    public int       ClassId         { get; set; }
    public int       SubjectId       { get; set; }
    /// <summary>1=Quiz,2=MonthlyTest,3=MidTerm,4=PreBoard,5=FinalExam</summary>
    public ExamType  ExamType        { get; set; }
    /// <summary>1=Primary,2=Middle,3=Matric,4=Inter</summary>
    public ClassGroup ClassGroup     { get; set; }
    public string?   Title           { get; set; }
    public int       TotalMarks      { get; set; }
    public int       PassMarks       { get; set; }
    public int?      DurationMinutes { get; set; }
    public string?   Instructions    { get; set; }
    public string?   SyllabusNote    { get; set; }
    public bool      IsDraft         { get; set; } = true;

    public List<CreateExamPaperSectionDto> Sections { get; set; } = new();
}

public class UpdateExamPaperDto
{
    public string?   Title           { get; set; }
    public int?      TotalMarks      { get; set; }
    public int?      PassMarks       { get; set; }
    public int?      DurationMinutes { get; set; }
    public string?   Instructions    { get; set; }
    public string?   SyllabusNote    { get; set; }
    public bool?     IsDraft         { get; set; }

    /// <summary>When provided, replaces all sections.</summary>
    public List<CreateExamPaperSectionDto>? Sections { get; set; }
}
