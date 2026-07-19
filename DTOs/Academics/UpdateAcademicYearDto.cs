using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Academics;

public class UpdateAcademicYearDto
{
    [Required, StringLength(50)]
    public string YearLabel { get; set; } = string.Empty;

    [Required] public DateOnly StartDate { get; set; }
    [Required] public DateOnly EndDate   { get; set; }

    public bool IsActive { get; set; }
}
