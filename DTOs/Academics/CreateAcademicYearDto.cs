using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Academics;

public class CreateAcademicYearDto
{
    [Required, StringLength(50)]
    public string YearLabel { get; set; } = string.Empty;

    [Required] public DateOnly StartDate { get; set; }
    [Required] public DateOnly EndDate   { get; set; }

    /// <summary>If true, this year becomes active and any previously-active year is deactivated.</summary>
    public bool IsActive { get; set; } = false;
}
