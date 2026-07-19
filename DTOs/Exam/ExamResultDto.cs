namespace SchoolManagement.API.DTOs.Exam;

/// <summary>Full result record returned to the caller.</summary>
public class ExamResultDto
{
    public int      ExamResultId     { get; set; }
    public int      ExamPaperId      { get; set; }
    public string   ExamTitle        { get; set; } = string.Empty;
    public int      StudentId        { get; set; }
    public string   StudentName      { get; set; } = string.Empty;
    public string   AdmissionNo      { get; set; } = string.Empty;
    public decimal  ObtainedMarks    { get; set; }
    public int      TotalMarks       { get; set; }
    public int      PassMarks        { get; set; }
    public decimal? Percentage       { get; set; }
    public string?  Grade            { get; set; }
    public bool     IsAbsent         { get; set; }
    public bool     IsPass           { get; set; }
    public int?     ClassRank        { get; set; }
    public string?  Remarks          { get; set; }
    public string?  SectionMarksJson { get; set; }
    public string?  EnteredByName    { get; set; }
    public DateTime? EnteredAt       { get; set; }
}

/// <summary>Used by teacher/admin to enter one student's result.</summary>
public class EnterExamResultDto
{
    public int     ExamPaperId      { get; set; }
    public int     StudentId        { get; set; }
    public decimal ObtainedMarks    { get; set; }
    /// <summary>Optional JSON of section-wise marks.</summary>
    public string? SectionMarksJson { get; set; }
    public bool    IsAbsent         { get; set; } = false;
    public string? Remarks          { get; set; }
}

/// <summary>Bulk result entry for a whole class at once.</summary>
public class BulkEnterExamResultDto
{
    public int ExamPaperId { get; set; }
    public List<StudentResultRowDto> Results { get; set; } = new();
}

public class StudentResultRowDto
{
    public int     StudentId        { get; set; }
    public decimal ObtainedMarks    { get; set; }
    public string? SectionMarksJson { get; set; }
    public bool    IsAbsent         { get; set; } = false;
    public string? Remarks          { get; set; }
}

/// <summary>Class result summary returned after publishing results.</summary>
public class ClassResultSummaryDto
{
    public int    ExamPaperId    { get; set; }
    public string ExamTitle      { get; set; } = string.Empty;
    public string ClassName      { get; set; } = string.Empty;
    public string SubjectName    { get; set; } = string.Empty;
    public int    TotalStudents  { get; set; }
    public int    Appeared       { get; set; }
    public int    Passed         { get; set; }
    public int    Failed         { get; set; }
    public int    Absent         { get; set; }
    public decimal? HighestMarks { get; set; }
    public decimal? LowestMarks  { get; set; }
    public decimal? AverageMarks { get; set; }
    public decimal? PassPercentage { get; set; }

    public List<ExamResultDto> StudentResults { get; set; } = new();
}

/// <summary>A student's result card (all exams in an academic year).</summary>
public class StudentResultCardDto
{
    public int    StudentId    { get; set; }
    public string StudentName  { get; set; } = string.Empty;
    public string AdmissionNo  { get; set; } = string.Empty;
    public string ClassName    { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;

    public List<SubjectResultRowDto> SubjectResults { get; set; } = new();
    public decimal? GrandTotal   { get; set; }
    public decimal? Percentage   { get; set; }
    public string?  OverallGrade { get; set; }
    public int?     ClassRank    { get; set; }
}

public class SubjectResultRowDto
{
    public string   SubjectName   { get; set; } = string.Empty;
    public string   ExamType      { get; set; } = string.Empty;
    public int      TotalMarks    { get; set; }
    public decimal  ObtainedMarks { get; set; }
    public decimal? Percentage    { get; set; }
    public string?  Grade         { get; set; }
    public bool     IsAbsent      { get; set; }
    public bool     IsPass        { get; set; }
}
