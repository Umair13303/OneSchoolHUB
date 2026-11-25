namespace SchoolManagement.API.DTOs.Dashboard;

/// <summary>
/// Aggregate stats for the admin/principal dashboard.
/// Semantics match the previous client-side reduction of GET /api/students?pageSize=0.
/// </summary>
public class DashboardSummaryDto
{
    public int TotalStudents { get; set; }
    public int MaleCount { get; set; }
    public int FemaleCount { get; set; }

    /// <summary>
    /// Enrollment class-name counts (ClassName only from latest active enrollment;
    /// missing/empty names become "Unknown"). Not pre-sliced — UI keeps top 8.
    /// </summary>
    public List<DashboardClassCountDto> ByClass { get; set; } = new();

    /// <summary>
    /// Last 7 school-local days, oldest first. Counts are distinct students
    /// (a student with Present or Late that day is Present; Absent only if
    /// they were marked absent and never present/late). Leave-only is omitted.
    /// </summary>
    public List<DashboardAttendanceDayDto> WeeklyAttendance { get; set; } = new();
}

public class DashboardClassCountDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DashboardAttendanceDayDto
{
    /// <summary>Short weekday, e.g. Mon.</summary>
    public string Day { get; set; } = string.Empty;
    /// <summary>School-local date, yyyy-MM-dd.</summary>
    public string Date { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
}
