namespace SchoolManagement.API.DTOs.Students;

/// <summary>Light-weight row for the student list page.</summary>
public class StudentListDto
{
    public int      StudentId       { get; set; }
    public string   AdmissionNo     { get; set; } = string.Empty;
    public string   FullName        { get; set; } = string.Empty;
    public DateOnly? DateOfBirth    { get; set; }
    public string?  Gender          { get; set; }

    /// <summary>The student's current active class (if any).</summary>
    public int?     ClassId         { get; set; }
    public string?  ClassName       { get; set; }
    public string?  Section         { get; set; }
    public int?     AcademicYearId  { get; set; }
    public string?  AcademicYearLabel { get; set; }

    public int?     PhotoFileId     { get; set; }
    public bool     IsActive        { get; set; }
}
