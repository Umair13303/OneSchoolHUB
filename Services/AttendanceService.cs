using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Attendance;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IAttendanceService
{
    Task<AttendanceRecordDto>        MarkSingleAsync(MarkAttendanceDto dto, int markedBy);
    Task<List<AttendanceRecordDto>>  BulkMarkAsync(BulkMarkAttendanceDto dto, int markedBy);
    Task<bool>                       UpdateAsync(int id, UpdateAttendanceDto dto, int updatedBy);
    Task<List<AttendanceRecordDto>>  GetForClassAsync(int classId, DateOnly date, int? periodId);
    Task<List<AttendanceRecordDto>>  GetForStudentAsync(int studentId, DateOnly? from, DateOnly? to);
    Task<AttendanceSummaryDto?>      GetSummaryAsync(int studentId, DateOnly? from, DateOnly? to);
    Task<List<AttendanceSummaryDto>> GetClassSummaryAsync(int classId, DateOnly from, DateOnly to);
}

public class AttendanceService : IAttendanceService
{
    private static readonly HashSet<string> ValidStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "Present", "Absent", "Late", "Leave" };

    private readonly AppDbContext _db;
    public AttendanceService(AppDbContext db) => _db = db;

    // ── Mark single ───────────────────────────────────────────────────────────
    public async Task<AttendanceRecordDto> MarkSingleAsync(MarkAttendanceDto dto, int markedBy)
    {
        Validate(dto.Status);
        await EnsureForeignKeysAsync(dto.ClassId, dto.PeriodId, dto.StudentId);

        var existing = await _db.Attendances.FirstOrDefaultAsync(a =>
            a.StudentId == dto.StudentId &&
            a.ClassId   == dto.ClassId   &&
            a.PeriodId  == dto.PeriodId  &&
            a.Date      == dto.Date);

        if (existing is not null)
            throw new InvalidOperationException(
                $"Attendance for student {dto.StudentId} on {dto.Date} period {dto.PeriodId} already exists. Use PUT to update.");

        var entity = new Attendance
        {
            StudentId = dto.StudentId,
            ClassId   = dto.ClassId,
            PeriodId  = dto.PeriodId,
            Date      = dto.Date,
            Status    = Normalise(dto.Status),
            Remarks   = dto.Remarks,
            MarkedBy  = markedBy,
            MarkedAt  = DateTime.UtcNow
        };
        _db.Attendances.Add(entity);
        await _db.SaveChangesAsync();

        return await FetchDtoAsync(entity.AttendanceId);
    }

    // ── Bulk mark (whole class, one period) ───────────────────────────────────
    public async Task<List<AttendanceRecordDto>> BulkMarkAsync(BulkMarkAttendanceDto dto, int markedBy)
    {
        if (dto.Entries.Count == 0)
            throw new ArgumentException("Entries list cannot be empty.");

        await EnsurePeriodExistsAsync(dto.PeriodId);
        if (!await _db.Classes.AnyAsync(c => c.ClassId == dto.ClassId))
            throw new ArgumentException($"Class {dto.ClassId} does not exist.");

        foreach (var e in dto.Entries)
            Validate(e.Status);

        var studentIds = dto.Entries.Select(e => e.StudentId).ToList();

        // Find which students already have attendance for this slot
        var existing = await _db.Attendances
            .Where(a => a.ClassId == dto.ClassId &&
                        a.PeriodId == dto.PeriodId &&
                        a.Date == dto.Date &&
                        studentIds.Contains(a.StudentId))
            .Select(a => a.StudentId)
            .ToListAsync();

        if (existing.Count > 0)
            throw new InvalidOperationException(
                $"Attendance already marked for student(s) [{string.Join(", ", existing)}] in this slot. Use PUT to update individual records.");

        var now = DateTime.UtcNow;
        var entities = dto.Entries.Select(e => new Attendance
        {
            StudentId = e.StudentId,
            ClassId   = dto.ClassId,
            PeriodId  = dto.PeriodId,
            Date      = dto.Date,
            Status    = Normalise(e.Status),
            Remarks   = e.Remarks,
            MarkedBy  = markedBy,
            MarkedAt  = now
        }).ToList();

        _db.Attendances.AddRange(entities);
        await _db.SaveChangesAsync();

        var ids = entities.Select(e => e.AttendanceId).ToList();
        return await _db.Attendances
            .AsNoTracking()
            .Where(a => ids.Contains(a.AttendanceId))
            .Include(a => a.Student)
            .Include(a => a.Class)
            .Include(a => a.Period)
            .Include(a => a.MarkedByUser)
            .Select(a => Map(a))
            .ToListAsync();
    }

    // ── Update single record ──────────────────────────────────────────────────
    public async Task<bool> UpdateAsync(int id, UpdateAttendanceDto dto, int updatedBy)
    {
        Validate(dto.Status);
        var entity = await _db.Attendances.FirstOrDefaultAsync(a => a.AttendanceId == id);
        if (entity is null) return false;

        entity.Status  = Normalise(dto.Status);
        entity.Remarks = dto.Remarks;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Queries ───────────────────────────────────────────────────────────────
    public async Task<List<AttendanceRecordDto>> GetForClassAsync(int classId, DateOnly date, int? periodId)
    {
        var q = _db.Attendances.AsNoTracking()
            .Where(a => a.ClassId == classId && a.Date == date);

        if (periodId.HasValue)
            q = q.Where(a => a.PeriodId == periodId.Value);

        return await q
            .Include(a => a.Student)
            .Include(a => a.Class)
            .Include(a => a.Period)
            .Include(a => a.MarkedByUser)
            .OrderBy(a => a.Period.PeriodNo)
            .ThenBy(a => a.Student.FirstName)
            .Select(a => Map(a))
            .ToListAsync();
    }

    public async Task<List<AttendanceRecordDto>> GetForStudentAsync(int studentId, DateOnly? from, DateOnly? to)
    {
        var q = _db.Attendances.AsNoTracking().Where(a => a.StudentId == studentId);
        if (from.HasValue) q = q.Where(a => a.Date >= from.Value);
        if (to.HasValue)   q = q.Where(a => a.Date <= to.Value);

        return await q
            .Include(a => a.Student)
            .Include(a => a.Class)
            .Include(a => a.Period)
            .Include(a => a.MarkedByUser)
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Period.PeriodNo)
            .Select(a => Map(a))
            .ToListAsync();
    }

    public async Task<AttendanceSummaryDto?> GetSummaryAsync(int studentId, DateOnly? from, DateOnly? to)
    {
        var student = await _db.Students.AsNoTracking()
            .FirstOrDefaultAsync(s => s.StudentId == studentId);
        if (student is null) return null;

        var q = _db.Attendances.AsNoTracking().Where(a => a.StudentId == studentId);
        if (from.HasValue) q = q.Where(a => a.Date >= from.Value);
        if (to.HasValue)   q = q.Where(a => a.Date <= to.Value);

        var records = await q.Select(a => a.Status).ToListAsync();

        return new AttendanceSummaryDto
        {
            StudentId   = student.StudentId,
            StudentName = $"{student.FirstName} {student.LastName}",
            AdmissionNo = student.AdmissionNo,
            TotalDays   = records.Count,
            Present     = records.Count(s => s.Equals("Present", StringComparison.OrdinalIgnoreCase)),
            Absent      = records.Count(s => s.Equals("Absent",  StringComparison.OrdinalIgnoreCase)),
            Late        = records.Count(s => s.Equals("Late",    StringComparison.OrdinalIgnoreCase))
        };
    }

    public async Task<List<AttendanceSummaryDto>> GetClassSummaryAsync(int classId, DateOnly from, DateOnly to)
    {
        var records = await _db.Attendances.AsNoTracking()
            .Where(a => a.ClassId == classId && a.Date >= from && a.Date <= to)
            .Include(a => a.Student)
            .ToListAsync();

        return records
            .GroupBy(a => a.StudentId)
            .Select(g =>
            {
                var statuses = g.Select(a => a.Status).ToList();
                var student  = g.First().Student;
                return new AttendanceSummaryDto
                {
                    StudentId   = g.Key,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    AdmissionNo = student.AdmissionNo,
                    TotalDays   = statuses.Count,
                    Present     = statuses.Count(s => s.Equals("Present", StringComparison.OrdinalIgnoreCase)),
                    Absent      = statuses.Count(s => s.Equals("Absent",  StringComparison.OrdinalIgnoreCase)),
                    Late        = statuses.Count(s => s.Equals("Late",    StringComparison.OrdinalIgnoreCase))
                };
            })
            .OrderBy(s => s.StudentName)
            .ToList();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private async Task<AttendanceRecordDto> FetchDtoAsync(int id) =>
        await _db.Attendances.AsNoTracking()
            .Where(a => a.AttendanceId == id)
            .Include(a => a.Student)
            .Include(a => a.Class)
            .Include(a => a.Period)
            .Include(a => a.MarkedByUser)
            .Select(a => Map(a))
            .FirstAsync();

    private async Task EnsureForeignKeysAsync(int classId, int periodId, int studentId)
    {
        if (!await _db.Classes.AnyAsync(c => c.ClassId == classId))
            throw new ArgumentException($"Class {classId} does not exist.");
        if (!await _db.Students.AnyAsync(s => s.StudentId == studentId))
            throw new ArgumentException($"Student {studentId} does not exist.");
        await EnsurePeriodExistsAsync(periodId);
    }

    private async Task EnsurePeriodExistsAsync(int periodId)
    {
        var period = await _db.Periods.FirstOrDefaultAsync(p => p.PeriodId == periodId);
        if (period is null)
            throw new ArgumentException($"Period {periodId} does not exist.");
        if (period.IsBreak)
            throw new ArgumentException("Cannot mark attendance for a break period.");
    }

    private static void Validate(string status)
    {
        if (!ValidStatuses.Contains(status))
            throw new ArgumentException($"Invalid status '{status}'. Must be Present, Absent, Late, or Leave.");
    }

    private static string Normalise(string status) =>
        char.ToUpper(status[0]) + status[1..].ToLower();

    private static AttendanceRecordDto Map(Attendance a) => new()
    {
        AttendanceId = a.AttendanceId,
        StudentId    = a.StudentId,
        StudentName  = a.Student    != null ? $"{a.Student.FirstName} {a.Student.LastName}" : string.Empty,
        AdmissionNo  = a.Student    != null ? a.Student.AdmissionNo : string.Empty,
        ClassId      = a.ClassId,
        ClassName    = a.Class      != null ? a.Class.ClassName  : string.Empty,
        Section      = a.Class      != null ? a.Class.Section    : null,
        PeriodId     = a.PeriodId,
        PeriodName   = a.Period     != null ? a.Period.PeriodName : string.Empty,
        Date         = a.Date,
        Status       = a.Status,
        Remarks      = a.Remarks,
        MarkedBy     = a.MarkedBy,
        MarkedByName = a.MarkedByUser != null ? a.MarkedByUser.FullName : string.Empty,
        MarkedAt     = a.MarkedAt
    };
}
