namespace SchoolManagement.API.DTOs.Academics;

/// <summary>One subject taught in one class by one teacher.</summary>
public class ClassSubjectDto
{
    public int    Id           { get; set; }
    public int    ClassId      { get; set; }
    public int    SubjectId    { get; set; }
    public string SubjectName  { get; set; } = string.Empty;
    public int?   TeacherId    { get; set; }
    public string TeacherName  { get; set; } = string.Empty;
    public bool   IsActive      { get; set; }
    public string ClassName     { get; set; } = string.Empty;
    public string? ClassSection { get; set; }
}
