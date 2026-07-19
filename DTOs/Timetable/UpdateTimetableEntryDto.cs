using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Timetable;

public class UpdateTimetableEntryDto
{
    [Required] public int ClassId        { get; set; }
    [Required] public int SubjectId      { get; set; }
    [Required] public int TeacherId      { get; set; }
    [Required] public int PeriodId       { get; set; }

    [Range(1, 7)] public int DayOfWeek { get; set; }

    [Required] public int AcademicYearId { get; set; }
}
