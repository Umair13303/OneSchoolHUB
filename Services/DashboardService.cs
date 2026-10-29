using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Dashboard;

namespace SchoolManagement.API.Services;

public interface IDashboardService
{
    /// <summary>
    /// Tenant-scoped student aggregates matching the legacy dashboard client math
    /// over GET /api/students?pageSize=0 (no IsActive / academic-year filters).
    /// </summary>
    Task<DashboardSummaryDto> GetSummaryAsync();
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<DashboardSummaryDto> GetSummaryAsync()
    {
        // Global Student filter: !IsDeleted + institute (or all institutes for superadmin).
        // Do not add IsActive or academic-year filters — parity with pageSize=0 list.
        var students = _db.Students.AsNoTracking();

        var totalStudents = await students.CountAsync();

        // Match JS: (gender?.toLowerCase() === 'male'|'female'). Null/empty/other excluded.
        var maleCount = await students.CountAsync(s =>
            s.Gender != null && s.Gender.ToLower() == "male");
        var femaleCount = await students.CountAsync(s =>
            s.Gender != null && s.Gender.ToLower() == "female");

        // Same latest-active-enrollment ClassName projection as StudentService.GetAllAsync.
        // Pull only class-name strings (not full StudentListDto), then group in memory so
        // null/empty both map to "Unknown" exactly like JS `className || 'Unknown'`.
        var classNames = await students
            .Select(st => st.Enrollments
                .Where(e => e.Status == "Active" && !e.IsDeleted)
                .OrderByDescending(e => e.EnrollmentDate)
                .Select(e => e.Class.ClassName)
                .FirstOrDefault())
            .ToListAsync();

        var byClass = classNames
            .GroupBy(n => string.IsNullOrEmpty(n) ? "Unknown" : n!)
            .Select(g => new DashboardClassCountDto
            {
                Name = g.Key,
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ThenBy(x => x.Name)
            .ToList();

        return new DashboardSummaryDto
        {
            TotalStudents = totalStudents,
            MaleCount = maleCount,
            FemaleCount = femaleCount,
            ByClass = byClass
        };
    }
}
