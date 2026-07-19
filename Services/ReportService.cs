using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Reports;

namespace SchoolManagement.API.Services;

public interface IReportService
{
    Task<AttendanceDailyReportDto?>   AttendanceDailyAsync(int classId, DateOnly date);
    Task<AttendanceMonthlyReportDto?> AttendanceMonthlyAsync(int classId, int month, int year);
    Task<StudentAttendanceReportDto?> StudentAttendanceAsync(int studentId, DateOnly? from, DateOnly? to);
    Task<HomeworkReportDto>           HomeworkAsync(int classId, DateOnly? from, DateOnly? to);
    Task<ClassEnrollmentReportDto?>   ClassEnrollmentAsync(int academicYearId);
    Task<TeacherWorkloadReportDto?>   TeacherWorkloadAsync(int academicYearId);
}

public class ReportService : IReportService
{
    private readonly AppDbContext _db;
    public ReportService(AppDbContext db) => _db = db;

    // ── 1. Attendance Daily ───────────────────────────────────────────────────
    public async Task<AttendanceDailyReportDto?> AttendanceDailyAsync(int classId, DateOnly date)
    {
        var cls = await _db.Classes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClassId == classId);
        if (cls is null) return null;

        // Students enrolled in this class (any active enrollment)
        var students = await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.ClassId == classId && e.Status == "Active" && !e.IsDeleted)
            .Include(e => e.Student)
            .OrderBy(e => e.Student.FirstName)
            .Select(e => e.Student)
            .ToListAsync();

        // Attendance records for the day
        var records = await _db.Attendances.AsNoTracking()
            .Where(a => a.ClassId == classId && a.Date == date)
            .Include(a => a.Period)
            .ToListAsync();

        // Non-break periods that have attendance records for this day
        var periodIds = records.Select(r => r.PeriodId).Distinct().ToList();
        var periods = await _db.Periods.AsNoTracking()
            .Where(p => periodIds.Contains(p.PeriodId) && !p.IsBreak)
            .OrderBy(p => p.PeriodNo)
            .ToListAsync();

        var periodDtos = periods.Select(p =>
        {
            var periodRecords = records.Where(r => r.PeriodId == p.PeriodId).ToList();
            var rows = students.Select(s =>
            {
                var rec = periodRecords.FirstOrDefault(r => r.StudentId == s.StudentId);
                return new AttendanceDailyRowDto
                {
                    StudentId   = s.StudentId,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    AdmissionNo = s.AdmissionNo,
                    Status      = rec?.Status ?? "Not Marked",
                    Remarks     = rec?.Remarks
                };
            }).ToList();

            return new AttendanceDailyPeriodDto
            {
                PeriodId   = p.PeriodId,
                PeriodName = p.PeriodName,
                Rows       = rows
            };
        }).ToList();

        // Aggregate across all periods for the header counts
        // Use first period (or all records) for a per-student view
        var allStatuses = records.Select(r => r.Status).ToList();

        return new AttendanceDailyReportDto
        {
            ClassId       = classId,
            ClassName     = cls.ClassName,
            Section       = cls.Section,
            Date          = date,
            TotalStudents = students.Count,
            Present       = allStatuses.Count(s => s.Equals("Present", StringComparison.OrdinalIgnoreCase)),
            Absent        = allStatuses.Count(s => s.Equals("Absent",  StringComparison.OrdinalIgnoreCase)),
            Late          = allStatuses.Count(s => s.Equals("Late",    StringComparison.OrdinalIgnoreCase)),
            NotMarked     = students.Count == 0 ? 0 : Math.Max(0, students.Count - records.Select(r => r.StudentId).Distinct().Count()),
            Periods       = periodDtos
        };
    }

    // ── 2. Attendance Monthly ─────────────────────────────────────────────────
    public async Task<AttendanceMonthlyReportDto?> AttendanceMonthlyAsync(int classId, int month, int year)
    {
        var cls = await _db.Classes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClassId == classId);
        if (cls is null) return null;

        var from = new DateOnly(year, month, 1);
        var to   = from.AddMonths(1).AddDays(-1);

        var students = await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.ClassId == classId && e.Status == "Active" && !e.IsDeleted)
            .Include(e => e.Student)
            .Select(e => e.Student)
            .ToListAsync();

        var records = await _db.Attendances.AsNoTracking()
            .Where(a => a.ClassId == classId && a.Date >= from && a.Date <= to)
            .ToListAsync();

        // Unique attendance days (dates that have any record for this class)
        var workingDays = records.Select(r => r.Date).Distinct().Count();

        var rows = students
            .OrderBy(s => s.FirstName)
            .Select(s =>
            {
                var studentRecords = records.Where(r => r.StudentId == s.StudentId).ToList();
                return new AttendanceMonthlyRowDto
                {
                    StudentId   = s.StudentId,
                    StudentName = $"{s.FirstName} {s.LastName}",
                    AdmissionNo = s.AdmissionNo,
                    Present     = studentRecords.Count(r => r.Status.Equals("Present", StringComparison.OrdinalIgnoreCase)),
                    Absent      = studentRecords.Count(r => r.Status.Equals("Absent",  StringComparison.OrdinalIgnoreCase)),
                    Late        = studentRecords.Count(r => r.Status.Equals("Late",    StringComparison.OrdinalIgnoreCase))
                };
            }).ToList();

        return new AttendanceMonthlyReportDto
        {
            ClassId     = classId,
            ClassName   = cls.ClassName,
            Section     = cls.Section,
            Month       = month,
            Year        = year,
            WorkingDays = workingDays,
            Students    = rows
        };
    }

    // ── 3. Student Attendance Report ──────────────────────────────────────────
    public async Task<StudentAttendanceReportDto?> StudentAttendanceAsync(int studentId, DateOnly? from, DateOnly? to)
    {
        var student = await _db.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student is null) return null;

        // Most recent active enrollment for class name
        var enrollment = await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.StudentId == studentId && e.Status == "Active" && !e.IsDeleted)
            .Include(e => e.Class)
            .OrderByDescending(e => e.EnrollmentDate)
            .FirstOrDefaultAsync();

        var q = _db.Attendances.AsNoTracking()
            .Where(a => a.StudentId == studentId);
        if (from.HasValue) q = q.Where(a => a.Date >= from.Value);
        if (to.HasValue)   q = q.Where(a => a.Date <= to.Value);

        var records = await q
            .Include(a => a.Period)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Period.PeriodNo)
            .ToListAsync();

        var days = records.Select(r => new StudentAttendanceDayDto
        {
            Date       = r.Date,
            PeriodName = r.Period?.PeriodName ?? string.Empty,
            Status     = r.Status,
            Remarks    = r.Remarks
        }).ToList();

        return new StudentAttendanceReportDto
        {
            StudentId    = student.StudentId,
            StudentName  = $"{student.FirstName} {student.LastName}",
            AdmissionNo  = student.AdmissionNo,
            ClassName    = enrollment?.Class?.ClassName ?? string.Empty,
            Section      = enrollment?.Class?.Section,
            From         = from,
            To           = to,
            TotalRecords = records.Count,
            Present      = records.Count(r => r.Status.Equals("Present", StringComparison.OrdinalIgnoreCase)),
            Absent       = records.Count(r => r.Status.Equals("Absent",  StringComparison.OrdinalIgnoreCase)),
            Late         = records.Count(r => r.Status.Equals("Late",    StringComparison.OrdinalIgnoreCase)),
            Days         = days
        };
    }

    // ── 4. Homework Completion Report ─────────────────────────────────────────
    public async Task<HomeworkReportDto> HomeworkAsync(int classId, DateOnly? from, DateOnly? to)
    {
        var cls = await _db.Classes.AsNoTracking()
            .FirstOrDefaultAsync(c => c.ClassId == classId);

        var totalStudents = await _db.StudentClassEnrollments.AsNoTracking()
            .CountAsync(e => e.ClassId == classId && e.Status == "Active" && !e.IsDeleted);

        var q = _db.Homeworks.AsNoTracking()
            .Where(h => h.ClassId == classId);
        if (from.HasValue) q = q.Where(h => h.AssignedDate >= from.Value);
        if (to.HasValue)   q = q.Where(h => h.AssignedDate <= to.Value);

        var homeworkList = await q
            .Include(h => h.Subject)
            .Include(h => h.Teacher)
            .Include(h => h.Submissions)
            .OrderByDescending(h => h.AssignedDate)
            .ToListAsync();

        var rows = homeworkList.Select(h =>
        {
            var subs = h.Submissions.Where(s => !s.IsDeleted).ToList();
            return new HomeworkReportRowDto
            {
                HomeworkId        = h.HomeworkId,
                Title             = h.Title,
                SubjectName       = h.Subject?.SubjectName ?? string.Empty,
                TeacherName       = h.Teacher?.FullName    ?? string.Empty,
                AssignedDate      = h.AssignedDate,
                DueDate           = h.DueDate,
                TotalStudents     = totalStudents,
                Submitted         = subs.Count(s => s.Status.Equals("Submitted", StringComparison.OrdinalIgnoreCase)
                                               || s.Status.Equals("Reviewed",  StringComparison.OrdinalIgnoreCase)),
                Reviewed          = subs.Count(s => s.Status.Equals("Reviewed",  StringComparison.OrdinalIgnoreCase)),
                Pending           = subs.Count(s => s.Status.Equals("Pending",   StringComparison.OrdinalIgnoreCase))
            };
        }).ToList();

        return new HomeworkReportDto
        {
            ClassId       = classId,
            ClassName     = cls?.ClassName   ?? string.Empty,
            Section       = cls?.Section,
            From          = from,
            To            = to,
            TotalHomework = homeworkList.Count,
            TotalStudents = totalStudents,
            Homework      = rows
        };
    }

    // ── 5. Class Enrollment Report ────────────────────────────────────────────
    public async Task<ClassEnrollmentReportDto?> ClassEnrollmentAsync(int academicYearId)
    {
        var academicYear = await _db.AcademicYears.AsNoTracking()
            .FirstOrDefaultAsync(y => y.AcademicYearId == academicYearId);
        if (academicYear is null) return null;

        var enrolledClassIds = await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.AcademicYearId == academicYearId && !e.IsDeleted)
            .Select(e => e.ClassId).Distinct().ToListAsync();

        var classes = await _db.Classes.AsNoTracking()
            .Where(c => enrolledClassIds.Contains(c.ClassId))
            .OrderBy(c => c.ClassName).ThenBy(c => c.Section)
            .ToListAsync();

        var allEnrollments = await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.AcademicYearId == academicYearId && !e.IsDeleted)
            .Include(e => e.Student)
            .ToListAsync();

        var classRows = classes.Select(cls =>
        {
            var enrollments = allEnrollments.Where(e => e.ClassId == cls.ClassId).ToList();
            var studentDtos = enrollments
                .OrderBy(e => e.Student.FirstName)
                .Select(e => new EnrolledStudentDto
                {
                    StudentId     = e.Student.StudentId,
                    StudentName   = $"{e.Student.FirstName} {e.Student.LastName}",
                    AdmissionNo   = e.Student.AdmissionNo,
                    Gender        = e.Student.Gender,
                    AdmissionDate = e.Student.AdmissionDate,
                    Status        = e.Status
                }).ToList();

            return new ClassEnrollmentRowDto
            {
                ClassId       = cls.ClassId,
                ClassName     = cls.ClassName,
                Section       = cls.Section,
                TotalStudents = enrollments.Count,
                Active        = enrollments.Count(e => e.Status == "Active"),
                Withdrawn     = enrollments.Count(e => e.Status == "Withdrawn"),
                Promoted      = enrollments.Count(e => e.Status == "Promoted"),
                Students      = studentDtos
            };
        }).ToList();

        return new ClassEnrollmentReportDto
        {
            AcademicYearId    = academicYearId,
            AcademicYearLabel = academicYear.YearLabel,
            TotalClasses      = classes.Count,
            TotalStudents     = allEnrollments.Select(e => e.StudentId).Distinct().Count(),
            Classes           = classRows
        };
    }

    // ── 6. Teacher Workload Report ────────────────────────────────────────────
    public async Task<TeacherWorkloadReportDto?> TeacherWorkloadAsync(int academicYearId)
    {
        var academicYear = await _db.AcademicYears.AsNoTracking()
            .FirstOrDefaultAsync(y => y.AcademicYearId == academicYearId);
        if (academicYear is null) return null;

        var timetableEntries = await _db.SchoolTimetables.AsNoTracking()
            .Where(t => t.AcademicYearId == academicYearId)
            .Include(t => t.Teacher)
            .Include(t => t.Class)
            .ToListAsync();

        var homeworkCounts = await _db.Homeworks.AsNoTracking()
            .Where(h => !h.IsDeleted)
            .GroupBy(h => h.TeacherId)
            .Select(g => new { TeacherId = g.Key, Count = g.Count() })
            .ToListAsync();

        var teacherRows = timetableEntries
            .GroupBy(t => t.TeacherId)
            .Select(g =>
            {
                var teacher    = g.First().Teacher;
                var hwCount    = homeworkCounts.FirstOrDefault(h => h.TeacherId == g.Key)?.Count ?? 0;
                var classNames = g.Select(t => t.Class != null
                    ? $"{t.Class.ClassName}{(t.Class.Section != null ? $" {t.Class.Section}" : "")}"
                    : string.Empty)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToList();

                return new TeacherWorkloadRowDto
                {
                    TeacherId               = g.Key,
                    TeacherName             = teacher?.FullName ?? string.Empty,
                    Email                   = teacher?.Email    ?? string.Empty,
                    TimetablePeriodsPerWeek = g.Count(),
                    HomeworkAssigned        = hwCount,
                    ClassesTeaching         = classNames
                };
            })
            .OrderBy(r => r.TeacherName)
            .ToList();

        return new TeacherWorkloadReportDto
        {
            AcademicYearId    = academicYearId,
            AcademicYearLabel = academicYear.YearLabel,
            Teachers          = teacherRows
        };
    }
}
