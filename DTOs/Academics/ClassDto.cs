namespace SchoolManagement.API.DTOs.Academics;

/// <summary>Response shape for class list/detail. Includes year label + assigned subject count for quick UI display.</summary>
public class ClassDto
{
    public int    ClassId         { get; set; }
    public string ClassName       { get; set; } = string.Empty;
    public string? Section        { get; set; }
    public bool   IsActive        { get; set; }

    public int?    InstituteId    { get; set; }
    public string? InstituteName  { get; set; }

    public int    SubjectCount    { get; set; }
    public List<ClassSubjectDto> Subjects { get; set; } = new();
}
