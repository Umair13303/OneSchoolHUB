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
            ByClass = byClass,
            WeeklyAttendance = await GetWeeklyAttendanceAsync()
        };
    }

    /// <summary>
    /// Distinct students per day for the last 7 days in Pakistan time.
    /// Attendance is stored per period, so a student is counted once:
    /// Present or Late wins over Absent; Leave-only is not plotted.
    /// </summary>
    private async Task<List<DashboardAttendanceDayDto>> GetWeeklyAttendanceAsync()
    {
        var today = PakistanToday();
        var from = today.AddDays(-6);

        var marks = await _db.Attendances.AsNoTracking()
            .Where(a => a.Date >= from && a.Date <= today)
            .Select(a => new { a.Date, a.StudentId, a.Status })
            .ToListAsync();

        var days = new List<DashboardAttendanceDayDto>(7);
        for (var date = from; date <= today; date = date.AddDays(1))
        {
            var present = 0;
            var absent = 0;
            foreach (var student in marks.Where(m => m.Date == date).GroupBy(m => m.StudentId))
            {
                var came = student.Any(m =>
                    m.Status.Equals("Present", StringComparison.OrdinalIgnoreCase) ||
                    m.Status.Equals("Late", StringComparison.OrdinalIgnoreCase));
                var away = student.Any(m =>
                    m.Status.Equals("Absent", StringComparison.OrdinalIgnoreCase));
                if (came) present++;
                else if (away) absent++;
            }

            days.Add(new DashboardAttendanceDayDto
            {
                Day = Weekday(date.DayOfWeek),
                Date = date.ToString("yyyy-MM-dd"),
                Present = present,
                Absent = absent
            });
        }

        return days;
    }

    private static DateOnly PakistanToday()
    {
        TimeZoneInfo tz;
        try
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
        }
        catch (TimeZoneNotFoundException)
        {
            tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Karachi");
        }

        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz));
    }

    private static string Weekday(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "Mon",
        DayOfWeek.Tuesday => "Tue",
        DayOfWeek.Wednesday => "Wed",
        DayOfWeek.Thursday => "Thu",
        DayOfWeek.Friday => "Fri",
        DayOfWeek.Saturday => "Sat",
        _ => "Sun"
    };
}
