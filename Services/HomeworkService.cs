using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Homework;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IHomeworkService
{
    Task<HomeworkDto>          CreateAsync(CreateHomeworkDto dto, int teacherId);
    Task<bool>                 UpdateAsync(int id, UpdateHomeworkDto dto, int updatedBy);
    Task<bool>                 SoftDeleteAsync(int id, int deletedBy);
    Task<HomeworkDto?>         GetByIdAsync(int id);
    Task<List<HomeworkDto>>    GetForClassAsync(int classId, DateOnly? date, int? subjectId);
    Task<List<HomeworkDto>>    GetForStudentAsync(int studentId, DateOnly? from, DateOnly? to);
    Task<SubmissionDto>        SubmitAsync(int homeworkId, SubmitHomeworkDto dto);
    Task<bool>                 ReviewAsync(int submissionId, ReviewSubmissionDto dto, int reviewedBy);
    Task<List<SubmissionDto>>  GetSubmissionsAsync(int homeworkId);
    Task<SubmissionDto?>       GetSubmissionByStudentAsync(int homeworkId, int studentId);
}

public class HomeworkService : IHomeworkService
{
    private static readonly HashSet<string> ValidReviewStatuses =
        new(StringComparer.OrdinalIgnoreCase) { "Submitted", "Reviewed" };

    private readonly AppDbContext _db;
    public HomeworkService(AppDbContext db) => _db = db;

    // ── CRUD ──────────────────────────────────────────────────────────────────
    public async Task<HomeworkDto> CreateAsync(CreateHomeworkDto dto, int teacherId)
    {
        await ValidateForeignKeysAsync(dto.ClassId, dto.SubjectId, teacherId);

        if (dto.DueDate < dto.AssignedDate)
            throw new ArgumentException("DueDate cannot be before AssignedDate.");

        var entity = new Homework
        {
            ClassId      = dto.ClassId,
            SubjectId    = dto.SubjectId,
            TeacherId    = teacherId,
            Title        = dto.Title.Trim(),
            Description  = dto.Description?.Trim(),
            AssignedDate = dto.AssignedDate,
            DueDate      = dto.DueDate,
            FileId       = dto.FileId,
            CreatedBy    = teacherId,
            CreatedAt    = DateTime.UtcNow
        };
        _db.Homeworks.Add(entity);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(entity.HomeworkId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateHomeworkDto dto, int updatedBy)
    {
        var entity = await _db.Homeworks.FirstOrDefaultAsync(h => h.HomeworkId == id);
        if (entity is null) return false;

        if (dto.DueDate < entity.AssignedDate)
            throw new ArgumentException("DueDate cannot be before AssignedDate.");

        entity.Title       = dto.Title.Trim();
        entity.Description = dto.Description?.Trim();
        entity.DueDate     = dto.DueDate;
        entity.FileId      = dto.FileId;
        entity.UpdatedBy   = updatedBy;
        entity.UpdatedAt   = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id, int deletedBy)
    {
        var entity = await _db.Homeworks.FirstOrDefaultAsync(h => h.HomeworkId == id);
        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Reads ─────────────────────────────────────────────────────────────────
    public async Task<HomeworkDto?> GetByIdAsync(int id) =>
        await _db.Homeworks.AsNoTracking()
            .Where(h => h.HomeworkId == id)
            .Include(h => h.Class).Include(h => h.Subject).Include(h => h.Teacher)
            .Include(h => h.Submissions)
            .Select(h => MapDto(h))
            .FirstOrDefaultAsync();

    public async Task<List<HomeworkDto>> GetForClassAsync(int classId, DateOnly? date, int? subjectId)
    {
        var q = _db.Homeworks.AsNoTracking().Where(h => h.ClassId == classId);
        if (date.HasValue)      q = q.Where(h => h.AssignedDate == date.Value);
        if (subjectId.HasValue) q = q.Where(h => h.SubjectId    == subjectId.Value);

        return await q
            .Include(h => h.Class).Include(h => h.Subject).Include(h => h.Teacher)
            .Include(h => h.Submissions)
            .OrderByDescending(h => h.AssignedDate)
            .Select(h => MapDto(h))
            .ToListAsync();
    }

    public async Task<List<HomeworkDto>> GetForStudentAsync(int studentId, DateOnly? from, DateOnly? to)
    {
        // Find the student's active class enrollments to know which classes they belong to
        var classIds = await _db.StudentClassEnrollments.AsNoTracking()
            .Where(e => e.StudentId == studentId && e.Status == "Active")
            .Select(e => e.ClassId)
            .ToListAsync();

        if (classIds.Count == 0) return [];

        var q = _db.Homeworks.AsNoTracking().Where(h => classIds.Contains(h.ClassId));
        if (from.HasValue) q = q.Where(h => h.AssignedDate >= from.Value);
        if (to.HasValue)   q = q.Where(h => h.AssignedDate <= to.Value);

        return await q
            .Include(h => h.Class).Include(h => h.Subject).Include(h => h.Teacher)
            .Include(h => h.Submissions)
            .OrderByDescending(h => h.AssignedDate)
            .Select(h => MapDto(h))
            .ToListAsync();
    }

    // ── Submissions ───────────────────────────────────────────────────────────
    public async Task<SubmissionDto> SubmitAsync(int homeworkId, SubmitHomeworkDto dto)
    {
        var homework = await _db.Homeworks.FirstOrDefaultAsync(h => h.HomeworkId == homeworkId);
        if (homework is null)
            throw new ArgumentException($"Homework {homeworkId} does not exist.");

        if (!await _db.Students.AnyAsync(s => s.StudentId == dto.StudentId))
            throw new ArgumentException($"Student {dto.StudentId} does not exist.");

        var existing = await _db.HomeworkSubmissions
            .FirstOrDefaultAsync(s => s.HomeworkId == homeworkId && s.StudentId == dto.StudentId);

        if (existing is not null)
            throw new InvalidOperationException(
                $"Student {dto.StudentId} has already submitted homework {homeworkId}.");

        var submission = new HomeworkSubmission
        {
            HomeworkId  = homeworkId,
            StudentId   = dto.StudentId,
            FileId      = dto.FileId,
            SubmittedAt = DateTime.UtcNow,
            Status      = "Submitted",
            CreatedAt   = DateTime.UtcNow
        };
        _db.HomeworkSubmissions.Add(submission);
        await _db.SaveChangesAsync();

        return await FetchSubmissionDtoAsync(submission.SubmissionId);
    }

    public async Task<bool> ReviewAsync(int submissionId, ReviewSubmissionDto dto, int reviewedBy)
    {
        if (!ValidReviewStatuses.Contains(dto.Status))
            throw new ArgumentException($"Invalid status '{dto.Status}'. Must be Submitted or Reviewed.");

        var submission = await _db.HomeworkSubmissions
            .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        if (submission is null) return false;

        submission.Status = dto.Status;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<SubmissionDto>> GetSubmissionsAsync(int homeworkId) =>
        await _db.HomeworkSubmissions.AsNoTracking()
            .Where(s => s.HomeworkId == homeworkId && !s.IsDeleted)
            .Include(s => s.Student)
            .Include(s => s.Homework)
            .OrderBy(s => s.Student.FirstName)
            .Select(s => MapSubmission(s))
            .ToListAsync();

    public async Task<SubmissionDto?> GetSubmissionByStudentAsync(int homeworkId, int studentId) =>
        await _db.HomeworkSubmissions.AsNoTracking()
            .Where(s => s.HomeworkId == homeworkId && s.StudentId == studentId && !s.IsDeleted)
            .Include(s => s.Student)
            .Include(s => s.Homework)
            .Select(s => MapSubmission(s))
            .FirstOrDefaultAsync();

    // ── Validation ────────────────────────────────────────────────────────────
    private async Task ValidateForeignKeysAsync(int classId, int subjectId, int teacherId)
    {
        if (!await _db.Classes.AnyAsync(c => c.ClassId == classId))
            throw new ArgumentException($"Class {classId} does not exist.");
        if (!await _db.Subjects.AnyAsync(s => s.SubjectId == subjectId))
            throw new ArgumentException($"Subject {subjectId} does not exist.");

        var teacher = await _db.Users.Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == teacherId && u.IsActive);
        if (teacher is null)
            throw new ArgumentException($"Teacher {teacherId} not found or inactive.");
        if (!string.Equals(teacher.Role.RoleName, "teacher", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"User {teacherId} is not a teacher.");
    }

    // ── Mapping ───────────────────────────────────────────────────────────────
    private async Task<SubmissionDto> FetchSubmissionDtoAsync(int id) =>
        await _db.HomeworkSubmissions.AsNoTracking()
            .Where(s => s.SubmissionId == id)
            .Include(s => s.Student)
            .Include(s => s.Homework)
            .Select(s => MapSubmission(s))
            .FirstAsync();

    private static HomeworkDto MapDto(Homework h) => new()
    {
        HomeworkId      = h.HomeworkId,
        ClassId         = h.ClassId,
        ClassName       = h.Class   != null ? h.Class.ClassName   : string.Empty,
        Section         = h.Class   != null ? h.Class.Section     : null,
        SubjectId       = h.SubjectId,
        SubjectName     = h.Subject != null ? h.Subject.SubjectName : string.Empty,
        TeacherId       = h.TeacherId,
        TeacherName     = h.Teacher != null ? h.Teacher.FullName  : string.Empty,
        Title           = h.Title,
        Description     = h.Description,
        AssignedDate    = h.AssignedDate,
        DueDate         = h.DueDate,
        FileId          = h.FileId,
        SubmissionCount = h.Submissions?.Count(s => !s.IsDeleted) ?? 0,
        CreatedAt       = h.CreatedAt
    };

    private static SubmissionDto MapSubmission(HomeworkSubmission s) => new()
    {
        SubmissionId   = s.SubmissionId,
        HomeworkId     = s.HomeworkId,
        HomeworkTitle  = s.Homework != null ? s.Homework.Title : string.Empty,
        StudentId      = s.StudentId,
        StudentName    = s.Student  != null ? $"{s.Student.FirstName} {s.Student.LastName}" : string.Empty,
        AdmissionNo    = s.Student  != null ? s.Student.AdmissionNo : string.Empty,
        SubmittedAt    = s.SubmittedAt,
        FileId         = s.FileId,
        Status         = s.Status,
        CreatedAt      = s.CreatedAt
    };
}
