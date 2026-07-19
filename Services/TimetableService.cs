using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Timetable;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ITimetableService
{
    Task<List<TimetableEntryDto>> GetForClassAsync(int classId, int? dayOfWeek, int? academicYearId);
    Task<TimetableEntryDto?>      GetByIdAsync(int id);
    Task<List<TimetableDayDto>>   GetForTeacherAsync(int teacherId, int? academicYearId);
    Task<TimetableEntryDto>       CreateAsync(CreateTimetableEntryDto dto, int createdBy);
    Task<bool>                    UpdateAsync(int id, UpdateTimetableEntryDto dto, int updatedBy);
    Task<bool>                    SoftDeleteAsync(int id, int deletedBy);
}

/// <summary>
/// Implements Module 5 (Timetable). Conflict checks:
///   1) The (Class, Period, Day, Year) slot is unique — a class can't have
///      two subjects in the same period.
///   2) The (Teacher, Period, Day, Year) slot is unique — a teacher can't
///      be in two classes at once.
///   3) The selected Period must not be a break.
/// </summary>
public class TimetableService : ITimetableService
{
    private readonly AppDbContext _db;
    public TimetableService(AppDbContext db) => _db = db;

    // ── Reads ─────────────────────────────────────────────────────────────
    public async Task<List<TimetableEntryDto>> GetForClassAsync(int classId, int? dayOfWeek, int? academicYearId)
    {
        var query = _db.SchoolTimetables.AsNoTracking().Where(t => t.ClassId == classId && !t.Period.IsBreak);
        if (dayOfWeek.HasValue)      query = query.Where(t => t.DayOfWeek == dayOfWeek.Value);
        if (academicYearId.HasValue) query = query.Where(t => t.AcademicYearId == academicYearId.Value);

        return await query
            .Include(t => t.Class).Include(t => t.Subject).Include(t => t.Teacher)
            .Include(t => t.Period).Include(t => t.AcademicYear)
            .OrderBy(t => t.DayOfWeek).ThenBy(t => t.Period.PeriodNo)
            .Select(t => Map(t))
            .ToListAsync();
    }

    public async Task<TimetableEntryDto?> GetByIdAsync(int id)
    {
        return await _db.SchoolTimetables.AsNoTracking()
            .Where(t => t.TimetableId == id)
            .Include(t => t.Class).Include(t => t.Subject).Include(t => t.Teacher)
            .Include(t => t.Period).Include(t => t.AcademicYear)
            .Select(t => Map(t))
            .FirstOrDefaultAsync();
    }

    public async Task<List<TimetableDayDto>> GetForTeacherAsync(int teacherId, int? academicYearId)
    {
        var query = _db.SchoolTimetables.AsNoTracking()
            .Where(t => t.TeacherId == teacherId && !t.Period.IsBreak);
        if (academicYearId.HasValue) query = query.Where(t => t.AcademicYearId == academicYearId.Value);

        var rows = await query
            .Include(t => t.Class).Include(t => t.Subject).Include(t => t.Teacher)
            .Include(t => t.Period).Include(t => t.AcademicYear)
            .OrderBy(t => t.DayOfWeek).ThenBy(t => t.Period.PeriodNo)
            .Select(t => Map(t))
            .ToListAsync();

        return rows
            .GroupBy(r => r.DayOfWeek)
            .OrderBy(g => g.Key)
            .Select(g => new TimetableDayDto
            {
                DayOfWeek = g.Key,
                DayName   = DayName(g.Key),
                Entries   = g.OrderBy(e => e.PeriodNo).ToList()
            })
            .ToList();
    }

    // ── Writes ────────────────────────────────────────────────────────────
    public async Task<TimetableEntryDto> CreateAsync(CreateTimetableEntryDto dto, int createdBy)
    {
        await ValidateForeignKeysAsync(dto.ClassId, dto.SubjectId, dto.TeacherId, dto.PeriodId, dto.AcademicYearId);
        await EnsureNoConflictAsync(dto.ClassId, dto.TeacherId, dto.PeriodId, dto.DayOfWeek, dto.AcademicYearId, excludeId: null);

        var entity = new SchoolTimetable
        {
            ClassId         = dto.ClassId,
            SubjectId       = dto.SubjectId,
            TeacherId       = dto.TeacherId,
            PeriodId        = dto.PeriodId,
            DayOfWeek       = dto.DayOfWeek,
            AcademicYearId  = dto.AcademicYearId,
            CreatedBy       = createdBy
        };
        _db.SchoolTimetables.Add(entity);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(entity.TimetableId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateTimetableEntryDto dto, int updatedBy)
    {
        var entity = await _db.SchoolTimetables.FirstOrDefaultAsync(t => t.TimetableId == id);
        if (entity is null) return false;

        await ValidateForeignKeysAsync(dto.ClassId, dto.SubjectId, dto.TeacherId, dto.PeriodId, dto.AcademicYearId);
        await EnsureNoConflictAsync(dto.ClassId, dto.TeacherId, dto.PeriodId, dto.DayOfWeek, dto.AcademicYearId, excludeId: id);

        entity.ClassId        = dto.ClassId;
        entity.SubjectId      = dto.SubjectId;
        entity.TeacherId      = dto.TeacherId;
        entity.PeriodId       = dto.PeriodId;
        entity.DayOfWeek      = dto.DayOfWeek;
        entity.AcademicYearId = dto.AcademicYearId;
        entity.UpdatedBy      = updatedBy;
        entity.UpdatedAt      = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id, int deletedBy)
    {
        var entity = await _db.SchoolTimetables.FirstOrDefaultAsync(t => t.TimetableId == id);
        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ──────────────────────────────────────────────────────────────────────
    // Validation
    // ──────────────────────────────────────────────────────────────────────

    private async Task ValidateForeignKeysAsync(int classId, int subjectId, int teacherId, int periodId, int academicYearId)
    {
        if (!await _db.Classes.AnyAsync(c => c.ClassId == classId))
            throw new ArgumentException($"Class {classId} does not exist.");
        if (!await _db.Subjects.AnyAsync(s => s.SubjectId == subjectId))
            throw new ArgumentException($"Subject {subjectId} does not exist.");
        if (!await _db.AcademicYears.AnyAsync(y => y.AcademicYearId == academicYearId && !y.IsDeleted))
            throw new ArgumentException($"AcademicYear {academicYearId} does not exist.");

        var period = await _db.Periods.FirstOrDefaultAsync(p => p.PeriodId == periodId && !p.IsDeleted);
        if (period is null)
            throw new ArgumentException($"Period {periodId} does not exist.");
        if (period.IsBreak)
            throw new ArgumentException("Cannot assign subjects to a break period.");

        var teacher = await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == teacherId && u.IsActive);
        if (teacher is null)
            throw new ArgumentException($"Teacher {teacherId} not found or inactive.");
        if (!string.Equals(teacher.Role.RoleName, "teacher", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"User {teacherId} is not a teacher (role: {teacher.Role.RoleName}).");
    }

    private async Task EnsureNoConflictAsync(int classId, int teacherId, int periodId, int dayOfWeek, int academicYearId, int? excludeId)
    {
        // Class slot already taken
        var classClash = await _db.SchoolTimetables.AnyAsync(t =>
            t.ClassId == classId &&
            t.PeriodId == periodId &&
            t.DayOfWeek == dayOfWeek &&
            t.AcademicYearId == academicYearId &&
            (!excludeId.HasValue || t.TimetableId != excludeId.Value));
        if (classClash)
            throw new InvalidOperationException("This class already has a subject scheduled in that period on that day.");

        // Teacher double-booked
        var teacherClash = await _db.SchoolTimetables.AnyAsync(t =>
            t.TeacherId == teacherId &&
            t.PeriodId == periodId &&
            t.DayOfWeek == dayOfWeek &&
            t.AcademicYearId == academicYearId &&
            (!excludeId.HasValue || t.TimetableId != excludeId.Value));
        if (teacherClash)
            throw new InvalidOperationException("That teacher is already teaching another class in the same period on that day.");
    }

    // ──────────────────────────────────────────────────────────────────────
    private static TimetableEntryDto Map(SchoolTimetable t) => new()
    {
        TimetableId       = t.TimetableId,
        ClassId           = t.ClassId,
        ClassName         = t.Class      != null ? t.Class.ClassName       : string.Empty,
        Section           = t.Class      != null ? t.Class.Section          : null,
        SubjectId         = t.SubjectId,
        SubjectName       = t.Subject    != null ? t.Subject.SubjectName    : string.Empty,
        TeacherId         = t.TeacherId,
        TeacherName       = t.Teacher    != null ? t.Teacher.FullName       : string.Empty,
        PeriodId          = t.PeriodId,
        PeriodNo          = t.Period     != null ? t.Period.PeriodNo        : 0,
        PeriodName        = t.Period     != null ? t.Period.PeriodName      : string.Empty,
        StartTime         = t.Period     != null ? t.Period.StartTime       : default,
        EndTime           = t.Period     != null ? t.Period.EndTime         : default,
        IsBreak           = t.Period     != null && t.Period.IsBreak,
        DayOfWeek         = t.DayOfWeek,
        AcademicYearId    = t.AcademicYearId,
        AcademicYearLabel = t.AcademicYear != null ? t.AcademicYear.YearLabel : null
    };

    private static string DayName(int day) => day switch
    {
        1 => "Monday",
        2 => "Tuesday",
        3 => "Wednesday",
        4 => "Thursday",
        5 => "Friday",
        6 => "Saturday",
        7 => "Sunday",
        _ => $"Day {day}"
    };
}
