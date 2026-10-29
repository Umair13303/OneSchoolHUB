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
}

public class DashboardClassCountDto
{
    public string Name { get; set; } = string.Empty;
    public int Count { get; set; }
}
