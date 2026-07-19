namespace SchoolManagement.API.DTOs.Academics;

public class SubjectDto
{
    public int    SubjectId   { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public bool   IsActive    { get; set; }

    public int?    InstituteId   { get; set; }
    public string? InstituteName { get; set; }
}
