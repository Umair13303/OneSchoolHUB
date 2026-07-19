namespace SchoolManagement.API.DTOs.Students;

/// <summary>Full student profile returned by <c>GET /api/students/{id}</c>.</summary>
public class StudentDetailDto
{
    public int      StudentId     { get; set; }
    public string   AdmissionNo   { get; set; } = string.Empty;
    public string   FirstName     { get; set; } = string.Empty;
    public string   LastName      { get; set; } = string.Empty;
    public DateOnly? DateOfBirth  { get; set; }
    public string?  Gender        { get; set; }
    public string?  BloodGroup    { get; set; }
    public string?  Religion      { get; set; }
    public string?  Nationality   { get; set; }
    public string?  Address       { get; set; }
    public string?  Phone         { get; set; }
    public string?  Email         { get; set; }
    public int?     PhotoFileId   { get; set; }
    public DateOnly  AdmissionDate { get; set; }
    public bool      IsActive      { get; set; }
    public DateOnly? LeavingDate   { get; set; }
    public string?   LeavingReason { get; set; }

    public int?     CurrentClassId         { get; set; }
    public string?  CurrentClassName       { get; set; }
    public string?  CurrentSection         { get; set; }
    public int?     CurrentAcademicYearId  { get; set; }
    public string?  CurrentAcademicYearLabel { get; set; }

    public List<GuardianDto> Guardians { get; set; } = new();
}
