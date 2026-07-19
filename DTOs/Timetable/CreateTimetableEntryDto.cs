using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Timetable;

public class CreateTimetableEntryDto
{
    [Required] public int ClassId        { get; set; }
    [Required] public int SubjectId      { get; set; }
    [Required] public int TeacherId      { get; set; }
    [Required] public int PeriodId       { get; set; }

    /// <summary>1 = Monday … 7 = Sunday. The seed uses 1=Mon..5=Fri.</summary>
    [Range(1, 7)] public int DayOfWeek { get; set; }

    [Required] public int AcademicYearId { get; set; }
}
