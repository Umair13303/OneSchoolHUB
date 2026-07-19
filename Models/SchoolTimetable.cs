namespace SchoolManagement.API.Models;

public class SchoolTimetable : BaseEntity
{
    public int TimetableId { get; set; }
    public int ClassId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int PeriodId { get; set; }
    public int DayOfWeek { get; set; }   // 1=Monday ... 5=Friday
    public int AcademicYearId { get; set; }

    public Class Class { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public User Teacher { get; set; } = null!;
    public Period Period { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
}
