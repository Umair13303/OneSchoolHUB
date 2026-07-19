using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Timetable;

public class CreatePeriodDto
{
    [Required] public int PeriodNo { get; set; }

    [Required, StringLength(50)]
    public string PeriodName { get; set; } = string.Empty;

    [Required] public TimeOnly StartTime { get; set; }
    [Required] public TimeOnly EndTime   { get; set; }

    public bool IsBreak  { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
