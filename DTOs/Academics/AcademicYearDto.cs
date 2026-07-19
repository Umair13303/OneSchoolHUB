namespace SchoolManagement.API.DTOs.Academics;

/// <summary>Response shape for academic year list/detail endpoints.</summary>
public class AcademicYearDto
{
    public int      AcademicYearId { get; set; }
    public string   YearLabel      { get; set; } = string.Empty;
    public DateOnly StartDate      { get; set; }
    public DateOnly EndDate        { get; set; }
    public bool     IsActive       { get; set; }
}
