namespace SchoolManagement.API.DTOs.Timetable;

/// <summary>View-model for the printable class timetable PDF.</summary>
public class ClassTimetablePdfModel
{
    public string  SchoolName  { get; set; } = string.Empty;
    public string? CampusName  { get; set; }
    public string  ClassLabel  { get; set; } = string.Empty;
    public string  PrintedBy   { get; set; } = "User";
    public DateTime PrintedOn  { get; set; } = DateTime.Now;

    /// <summary>Day numbers to include (1=Mon … 7=Sun).</summary>
    public List<int> Days { get; set; } = new();

    public List<ClassTimetablePdfPeriod> Periods { get; set; } = new();
}

public class ClassTimetablePdfPeriod
{
    public int    PeriodNo  { get; set; }
    public string StartTime { get; set; } = string.Empty;
    public string EndTime   { get; set; } = string.Empty;
    public bool   IsBreak   { get; set; }

    /// <summary>Key = DayOfWeek (1–7). Null = free slot.</summary>
    public Dictionary<int, ClassTimetablePdfCell?> Cells { get; set; } = new();
}

public class ClassTimetablePdfCell
{
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
}
