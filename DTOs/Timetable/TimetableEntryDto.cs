namespace SchoolManagement.API.DTOs.Timetable;

/// <summary>
/// One row of the timetable — a subject taught to a class by a teacher in a
/// specific period on a specific day. Includes the related labels for display.
/// </summary>
public class TimetableEntryDto
{
    public int    TimetableId    { get; set; }

    public int    ClassId        { get; set; }
    public string ClassName      { get; set; } = string.Empty;
    public string? Section       { get; set; }

    public int    SubjectId      { get; set; }
    public string SubjectName    { get; set; } = string.Empty;

    public int    TeacherId      { get; set; }
    public string TeacherName    { get; set; } = string.Empty;

    public int    PeriodId       { get; set; }
    public int    PeriodNo       { get; set; }
    public string PeriodName     { get; set; } = string.Empty;
    public TimeOnly StartTime    { get; set; }
    public TimeOnly EndTime      { get; set; }
    public bool   IsBreak        { get; set; }

    /// <summary>1 = Monday … 5 = Friday (matches the seed convention).</summary>
    public int    DayOfWeek      { get; set; }

    public int    AcademicYearId { get; set; }
    public string? AcademicYearLabel { get; set; }
}
