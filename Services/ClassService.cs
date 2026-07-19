using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IClassService
{
    Task<List<ClassDto>> GetAllAsync(int? academicYearId = null);
    Task<ClassDto?>      GetByIdAsync(int id);
    Task<ClassDto>       CreateAsync(CreateClassDto dto, int createdBy);
    Task<bool>           UpdateAsync(int id, UpdateClassDto dto, int updatedBy);
    Task<bool>           DeleteAsync(int id, int deletedBy);
    Task<ClassSubjectDto> AssignTeacherAsync(int classId, AssignTeacherDto dto, int createdBy);
    Task<List<ClassSubjectDto>> GetSubjectsForClassAsync(int classId);
    Task<bool>           RemoveSubjectAsync(int classId, int classSubjectId, int deletedBy);
    Task<ClassSubjectDto?> ToggleSubjectStatusAsync(int classId, int classSubjectId, int updatedBy);
    Task<List<ClassSubjectDto>> GetAssignmentsByTeacherAsync(int teacherId);
    Task<ClassSubjectDto?> UnassignTeacherAsync(int classId, int classSubjectId, int updatedBy);
}

public class ClassService : IClassService
{
    private readonly AppDbContext _db;
    private readonly ITenantContext _tenant;
    public ClassService(AppDbContext db, ITenantContext tenant) { _db = db; _tenant = tenant; }

    public async Task<List<ClassDto>> GetAllAsync(int? academicYearId)
    {
        var classes = await _db.Classes
            .AsNoTracking()
            .Include(c => c.ClassSubjects).ThenInclude(cs => cs.Subject)
            .Include(c => c.ClassSubjects).ThenInclude(cs => cs.Teacher)
            .OrderBy(c => c.ClassName).ThenBy(c => c.Section)
            .ToListAsync();

        var instituteNames = await GetInstituteNamesAsync(classes.Select(c => c.InstituteId));
        return classes.Select(c => MapToDto(c, instituteNames)).ToList();
    }

    public async Task<ClassDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Classes
            .AsNoTracking()
            .Where(c => c.ClassId == id)
            .Include(c => c.ClassSubjects).ThenInclude(cs => cs.Subject)
            .Include(c => c.ClassSubjects).ThenInclude(cs => cs.Teacher)
            .FirstOrDefaultAsync();
        if (entity is null) return null;

        var instituteNames = await GetInstituteNamesAsync(new[] { entity.InstituteId });
        return MapToDto(entity, instituteNames);
    }

    public async Task<ClassDto> CreateAsync(CreateClassDto dto, int createdBy)
    {
        var dupe = await _db.Classes.AnyAsync(c =>
            c.ClassName == dto.ClassName &&
            c.Section   == dto.Section);
        if (dupe)
            throw new InvalidOperationException("A class with the same name + section already exists.");

        int? instituteId = null;
        if (_tenant.IsSuperAdmin)
        {
            if (!dto.InstituteId.HasValue)
                throw new ArgumentException("InstituteId is required when creating a class as superadmin.");
            var instituteExists = await _db.Institutes.AnyAsync(i => i.InstituteId == dto.InstituteId.Value);
            if (!instituteExists)
                throw new ArgumentException($"Institute {dto.InstituteId} does not exist.");
            instituteId = dto.InstituteId;
        }

        var entity = new Class
        {
            ClassName   = dto.ClassName,
            Section     = dto.Section,
            IsActive    = dto.IsActive,
            InstituteId = instituteId,
            CreatedBy   = createdBy
        };
        _db.Classes.Add(entity);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(entity.ClassId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateClassDto dto, int updatedBy)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == id);
        if (entity is null) return false;

        var dupe = await _db.Classes.AnyAsync(c =>
            c.ClassName == dto.ClassName &&
            c.Section   == dto.Section &&
            c.ClassId   != id);
        if (dupe)
            throw new InvalidOperationException("Another class with the same name + section already exists.");

        entity.ClassName = dto.ClassName;
        entity.Section   = dto.Section;
        entity.IsActive  = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        if (_tenant.IsSuperAdmin && dto.InstituteId.HasValue)
        {
            var instituteExists = await _db.Institutes.AnyAsync(i => i.InstituteId == dto.InstituteId.Value);
            if (!instituteExists)
                throw new ArgumentException($"Institute {dto.InstituteId} does not exist.");
            entity.InstituteId = dto.InstituteId;
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int deletedBy)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(c => c.ClassId == id);
        if (entity is null) return false;

        var hasEnrollments = await _db.StudentClassEnrollments.AnyAsync(e => e.ClassId == id);
        if (hasEnrollments)
            throw new InvalidOperationException("Cannot delete a class that has student enrollments.");

        entity.IsDeleted = true;
        entity.IsActive  = false;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ClassSubjectDto> AssignTeacherAsync(int classId, AssignTeacherDto dto, int createdBy)
    {
        var classExists = await _db.Classes.AnyAsync(c => c.ClassId == classId);
        if (!classExists) throw new ArgumentException($"Class {classId} does not exist.");

        var subjectExists = await _db.Subjects.AnyAsync(s => s.SubjectId == dto.SubjectId);
        if (!subjectExists) throw new ArgumentException($"Subject {dto.SubjectId} does not exist.");

        // Teacher is optional — validate only when provided
        if (dto.TeacherId.HasValue && dto.TeacherId.Value > 0)
        {
            var teacher = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == dto.TeacherId.Value && u.IsActive);
            if (teacher is null)
                throw new ArgumentException($"Teacher {dto.TeacherId} not found or inactive.");
            if (!string.Equals(teacher.Role.RoleName, "teacher", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException($"User {dto.TeacherId} is not a teacher (role: {teacher.Role.RoleName}).");
        }

        // Upsert the (Class, Subject) tuple — replace the teacher if one was already assigned.
        var existing = await _db.ClassSubjects
            .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.SubjectId == dto.SubjectId);

        int? teacherId = (dto.TeacherId.HasValue && dto.TeacherId.Value > 0) ? dto.TeacherId : null;

        if (existing is null)
        {
            existing = new ClassSubject
            {
                ClassId   = classId,
                SubjectId = dto.SubjectId,
                TeacherId = teacherId,
                IsActive  = true,
                CreatedBy = createdBy
            };
            _db.ClassSubjects.Add(existing);
        }
        else
        {
            if (teacherId.HasValue) existing.TeacherId = teacherId;
            existing.IsActive  = true;
            existing.UpdatedBy = createdBy;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();

        return await _db.ClassSubjects
            .AsNoTracking()
            .Where(cs => cs.Id == existing.Id)
            .Include(cs => cs.Subject)
            .Include(cs => cs.Teacher)
            .Include(cs => cs.Class)
            .Select(cs => new ClassSubjectDto
            {
                Id           = cs.Id,
                ClassId      = cs.ClassId,
                SubjectId    = cs.SubjectId,
                SubjectName  = cs.Subject.SubjectName,
                TeacherId    = cs.TeacherId,
                TeacherName  = cs.Teacher != null ? cs.Teacher.FullName : string.Empty,
                IsActive     = cs.IsActive,
                ClassName    = cs.Class.ClassName,
                ClassSection = cs.Class.Section
            })
            .FirstAsync();
    }

    public async Task<List<ClassSubjectDto>> GetSubjectsForClassAsync(int classId)
    {
        return await _db.ClassSubjects
            .AsNoTracking()
            .Where(cs => cs.ClassId == classId)
            .Include(cs => cs.Subject)
            .Include(cs => cs.Teacher)
            .OrderBy(cs => cs.Subject.SubjectName)
            .Select(cs => new ClassSubjectDto
            {
                Id          = cs.Id,
                ClassId     = cs.ClassId,
                SubjectId   = cs.SubjectId,
                SubjectName = cs.Subject.SubjectName,
                TeacherId   = cs.TeacherId,
                TeacherName = cs.Teacher != null ? cs.Teacher.FullName : string.Empty,
                IsActive    = cs.IsActive
            })
            .ToListAsync();
    }

    public async Task<bool> RemoveSubjectAsync(int classId, int classSubjectId, int deletedBy)
    {
        var cs = await _db.ClassSubjects
            .FirstOrDefaultAsync(x => x.Id == classSubjectId && x.ClassId == classId);
        if (cs is null) return false;
        _db.ClassSubjects.Remove(cs);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<ClassSubjectDto?> ToggleSubjectStatusAsync(int classId, int classSubjectId, int updatedBy)
    {
        var cs = await _db.ClassSubjects
            .Include(x => x.Subject)
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == classSubjectId && x.ClassId == classId);
        if (cs is null) return null;
        cs.IsActive  = !cs.IsActive;
        cs.UpdatedBy = updatedBy;
        cs.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return new ClassSubjectDto
        {
            Id          = cs.Id,
            ClassId     = cs.ClassId,
            SubjectId   = cs.SubjectId,
            SubjectName = cs.Subject.SubjectName,
            TeacherId   = cs.TeacherId,
            TeacherName = cs.Teacher != null ? cs.Teacher.FullName : string.Empty,
            IsActive    = cs.IsActive
        };
    }

    public async Task<ClassSubjectDto?> UnassignTeacherAsync(int classId, int classSubjectId, int updatedBy)
    {
        var cs = await _db.ClassSubjects
            .Include(x => x.Subject)
            .Include(x => x.Class)
            .FirstOrDefaultAsync(x => x.Id == classSubjectId && x.ClassId == classId);
        if (cs is null) return null;
        cs.TeacherId = null;
        cs.UpdatedBy = updatedBy;
        cs.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return new ClassSubjectDto
        {
            Id           = cs.Id,
            ClassId      = cs.ClassId,
            SubjectId    = cs.SubjectId,
            SubjectName  = cs.Subject.SubjectName,
            TeacherId    = null,
            TeacherName  = string.Empty,
            IsActive     = cs.IsActive,
            ClassName    = cs.Class.ClassName,
            ClassSection = cs.Class.Section
        };
    }

    public async Task<List<ClassSubjectDto>> GetAssignmentsByTeacherAsync(int teacherId)
    {
        return await _db.ClassSubjects
            .AsNoTracking()
            .Where(cs => cs.TeacherId == teacherId)
            .Include(cs => cs.Class)
            .Include(cs => cs.Subject)
            .Include(cs => cs.Teacher)
            .OrderBy(cs => cs.Class.ClassName).ThenBy(cs => cs.Subject.SubjectName)
            .Select(cs => new ClassSubjectDto
            {
                Id          = cs.Id,
                ClassId     = cs.ClassId,
                SubjectId   = cs.SubjectId,
                SubjectName = cs.Subject.SubjectName,
                TeacherId   = cs.TeacherId,
                TeacherName = cs.Teacher != null ? cs.Teacher.FullName : string.Empty,
                IsActive    = cs.IsActive,
                ClassName   = cs.Class.ClassName,
                ClassSection = cs.Class.Section
            })
            .ToListAsync();
    }

    // ──────────────────────────────────────────────────────────────────────
    private async Task<Dictionary<int, string>> GetInstituteNamesAsync(IEnumerable<int?> instituteIds)
    {
        var ids = instituteIds.Where(i => i.HasValue).Select(i => i!.Value).Distinct().ToList();
        if (ids.Count == 0) return new Dictionary<int, string>();
        return await _db.Institutes
            .AsNoTracking()
            .Where(i => ids.Contains(i.InstituteId))
            .ToDictionaryAsync(i => i.InstituteId, i => i.Name);
    }

    private static ClassDto MapToDto(Class c, Dictionary<int, string> instituteNames) => new()
    {
        ClassId       = c.ClassId,
        ClassName     = c.ClassName,
        Section       = c.Section,
        IsActive      = c.IsActive,
        InstituteId   = c.InstituteId,
        InstituteName = c.InstituteId.HasValue && instituteNames.TryGetValue(c.InstituteId.Value, out var n) ? n : null,
        SubjectCount      = c.ClassSubjects.Count,
        Subjects          = c.ClassSubjects.Select(cs => new ClassSubjectDto
        {
            Id          = cs.Id,
            ClassId     = cs.ClassId,
            SubjectId   = cs.SubjectId,
            SubjectName = cs.Subject != null ? cs.Subject.SubjectName : string.Empty,
            TeacherId   = cs.TeacherId,
            TeacherName = cs.Teacher != null ? cs.Teacher.FullName : string.Empty,
            IsActive    = cs.IsActive
        }).ToList()
    };
}
