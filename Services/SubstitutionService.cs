using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Timetable;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ISubstitutionService
{
    /// <summary>Returns all timetable slots for the given date (DayOfWeek derived from date),
    /// enriched with any existing substitution for that date.</summary>
    Task<List<DaySlotDto>> GetDaySlotsAsync(DateOnly date, int? classId);

    Task<List<SubstitutionDto>> GetByDateRangeAsync(DateOnly from, DateOnly to, int? classId);
    Task<SubstitutionDto>       CreateAsync(CreateSubstitutionDto dto, int createdBy);
    Task<SubstitutionDto>       UpdateAsync(int id, UpdateSubstitutionDto dto, int updatedBy);
    Task                        DeleteAsync(int id);
}

public class SubstitutionService : ISubstitutionService
{
    private static readonly string[] DayNames = ["", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];
    private readonly AppDbContext _db;
    public SubstitutionService(AppDbContext db) => _db = db;

    public async Task<List<DaySlotDto>> GetDaySlotsAsync(DateOnly date, int? classId)
    {
        // DayOfWeek: Sunday=0 in .NET → map to 7; Monday=1 … Saturday=6
        var dow = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;

        var query = _db.SchoolTimetables
            .AsNoTracking()
            .Where(t => t.DayOfWeek == dow);

        if (classId.HasValue)
            query = query.Where(t => t.ClassId == classId.Value);

        var entries = await query
            .Include(t => t.Period)
            .Include(t => t.Subject)
            .Include(t => t.Class)
            .Include(t => t.Teacher)
            .OrderBy(t => t.Period.PeriodNo)
            .ToListAsync();

        // Load any substitutions already set for this date
        var timetableIds = entries.Select(e => e.TimetableId).ToList();
        var subs = await _db.TimetableSubstitutions
            .AsNoTracking()
            .Where(s => timetableIds.Contains(s.TimetableId) && s.Date == date)
            .Include(s => s.SubstituteTeacher)
            .ToListAsync();

        var subMap = subs.ToDictionary(s => s.TimetableId);

        return entries.Select(t =>
        {
            subMap.TryGetValue(t.TimetableId, out var sub);
            return new DaySlotDto
            {
                TimetableId           = t.TimetableId,
                PeriodId              = t.PeriodId,
                PeriodNo              = t.Period.PeriodNo,
                PeriodName            = t.Period.PeriodName,
                StartTime             = t.Period.StartTime.ToString("HH:mm"),
                EndTime               = t.Period.EndTime.ToString("HH:mm"),
                ClassId               = t.ClassId,
                ClassName             = t.Class.ClassName,
                Section               = t.Class.Section ?? "",
                SubjectId             = t.SubjectId,
                SubjectName           = t.Subject.SubjectName,
                OriginalTeacherId     = t.TeacherId,
                OriginalTeacherName   = t.Teacher.FullName,
                SubstitutionId        = sub?.SubstitutionId,
                SubstituteTeacherId   = sub?.SubstituteTeacherId,
                SubstituteTeacherName = sub?.SubstituteTeacher.FullName ?? "",
                Reason                = sub?.Reason ?? "",
            };
        }).ToList();
    }

    public async Task<List<SubstitutionDto>> GetByDateRangeAsync(DateOnly from, DateOnly to, int? classId)
    {
        var query = _db.TimetableSubstitutions
            .AsNoTracking()
            .Where(s => s.Date >= from && s.Date <= to);

        if (classId.HasValue)
            query = query.Where(s => s.Timetable.ClassId == classId.Value);

        return await query
            .Include(s => s.SubstituteTeacher)
            .Include(s => s.Timetable).ThenInclude(t => t.Teacher)
            .Include(s => s.Timetable).ThenInclude(t => t.Class)
            .Include(s => s.Timetable).ThenInclude(t => t.Subject)
            .Include(s => s.Timetable).ThenInclude(t => t.Period)
            .OrderBy(s => s.Date).ThenBy(s => s.Timetable.Period.PeriodNo)
            .Select(s => ToDto(s))
            .ToListAsync();
    }

    public async Task<SubstitutionDto> CreateAsync(CreateSubstitutionDto dto, int createdBy)
    {
        if (!DateOnly.TryParse(dto.Date, out var date))
            throw new ArgumentException("Invalid date format. Use yyyy-MM-dd.");

        var timetable = await _db.SchoolTimetables
            .Include(t => t.Teacher).Include(t => t.Class)
            .Include(t => t.Subject).Include(t => t.Period)
            .FirstOrDefaultAsync(t => t.TimetableId == dto.TimetableId)
            ?? throw new KeyNotFoundException("Timetable entry not found.");

        var sub = await _db.Users.FirstOrDefaultAsync(u => u.UserId == dto.SubstituteTeacherId && !u.IsDeleted)
            ?? throw new KeyNotFoundException("Substitute teacher not found.");

        var existing = await _db.TimetableSubstitutions
            .FirstOrDefaultAsync(s => s.TimetableId == dto.TimetableId && s.Date == date);
        if (existing != null)
            throw new InvalidOperationException("A substitution already exists for this slot on this date.");

        var entity = new TimetableSubstitution
        {
            TimetableId         = dto.TimetableId,
            Date                = date,
            SubstituteTeacherId = dto.SubstituteTeacherId,
            Reason              = dto.Reason,
            CreatedBy           = createdBy,
            CreatedAt           = DateTime.UtcNow,
        };
        _db.TimetableSubstitutions.Add(entity);
        await _db.SaveChangesAsync();

        entity.SubstituteTeacher = sub;
        entity.Timetable = timetable;
        return ToDto(entity);
    }

    public async Task<SubstitutionDto> UpdateAsync(int id, UpdateSubstitutionDto dto, int updatedBy)
    {
        var entity = await _db.TimetableSubstitutions
            .Include(s => s.SubstituteTeacher)
            .Include(s => s.Timetable).ThenInclude(t => t.Teacher)
            .Include(s => s.Timetable).ThenInclude(t => t.Class)
            .Include(s => s.Timetable).ThenInclude(t => t.Subject)
            .Include(s => s.Timetable).ThenInclude(t => t.Period)
            .FirstOrDefaultAsync(s => s.SubstitutionId == id)
            ?? throw new KeyNotFoundException("Substitution not found.");

        var sub = await _db.Users.FirstOrDefaultAsync(u => u.UserId == dto.SubstituteTeacherId && !u.IsDeleted)
            ?? throw new KeyNotFoundException("Substitute teacher not found.");

        entity.SubstituteTeacherId = dto.SubstituteTeacherId;
        entity.SubstituteTeacher   = sub;
        entity.Reason              = dto.Reason;
        entity.UpdatedBy           = updatedBy;
        entity.UpdatedAt           = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ToDto(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _db.TimetableSubstitutions.FirstOrDefaultAsync(s => s.SubstitutionId == id)
            ?? throw new KeyNotFoundException("Substitution not found.");
        entity.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    private static SubstitutionDto ToDto(TimetableSubstitution s) => new()
    {
        SubstitutionId        = s.SubstitutionId,
        TimetableId           = s.TimetableId,
        Date                  = s.Date.ToString("yyyy-MM-dd"),
        OriginalTeacherId     = s.Timetable.TeacherId,
        OriginalTeacherName   = s.Timetable.Teacher.FullName,
        SubstituteTeacherId   = s.SubstituteTeacherId,
        SubstituteTeacherName = s.SubstituteTeacher.FullName,
        ClassId               = s.Timetable.ClassId,
        ClassName             = s.Timetable.Class.ClassName,
        Section               = s.Timetable.Class.Section ?? "",
        SubjectId             = s.Timetable.SubjectId,
        SubjectName           = s.Timetable.Subject.SubjectName,
        PeriodId              = s.Timetable.PeriodId,
        PeriodNo              = s.Timetable.Period.PeriodNo,
        PeriodName            = s.Timetable.Period.PeriodName,
        StartTime             = s.Timetable.Period.StartTime.ToString("HH:mm"),
        EndTime               = s.Timetable.Period.EndTime.ToString("HH:mm"),
        DayOfWeek             = s.Timetable.DayOfWeek,
        Reason                = s.Reason,
    };
}
