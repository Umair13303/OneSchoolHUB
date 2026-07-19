using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ISubjectService
{
    Task<List<SubjectDto>> GetAllAsync();
    Task<SubjectDto?>      GetByIdAsync(int id);
    Task<SubjectDto>       CreateAsync(CreateSubjectDto dto, int createdBy);
    Task<bool>             UpdateAsync(int id, UpdateSubjectDto dto, int updatedBy);
    Task<bool>             DeleteAsync(int id, int deletedBy);
}

public class SubjectService : ISubjectService
{
    private readonly AppDbContext _db;
    private readonly ITenantContext _tenant;
    public SubjectService(AppDbContext db, ITenantContext tenant) { _db = db; _tenant = tenant; }

    public async Task<List<SubjectDto>> GetAllAsync()
    {
        var subjects = await _db.Subjects
            .AsNoTracking()
            .OrderBy(s => s.SubjectName)
            .ToListAsync();

        var instituteNames = await GetInstituteNamesAsync(subjects.Select(s => s.InstituteId));
        return subjects.Select(s => MapToDto(s, instituteNames)).ToList();
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.SubjectId == id);
        if (entity is null) return null;

        var instituteNames = await GetInstituteNamesAsync(new[] { entity.InstituteId });
        return MapToDto(entity, instituteNames);
    }

    public async Task<SubjectDto> CreateAsync(CreateSubjectDto dto, int createdBy)
    {
        var exists = await _db.Subjects.AnyAsync(s => s.SubjectName == dto.SubjectName);
        if (exists)
            throw new InvalidOperationException($"A subject named '{dto.SubjectName}' already exists.");

        int? instituteId = null;
        if (_tenant.IsSuperAdmin)
        {
            if (!dto.InstituteId.HasValue)
                throw new ArgumentException("InstituteId is required when creating a subject as superadmin.");
            var instituteExists = await _db.Institutes.AnyAsync(i => i.InstituteId == dto.InstituteId.Value);
            if (!instituteExists)
                throw new ArgumentException($"Institute {dto.InstituteId} does not exist.");
            instituteId = dto.InstituteId;
        }

        var entity = new Subject
        {
            SubjectName = dto.SubjectName,
            IsActive    = dto.IsActive,
            InstituteId = instituteId,
            CreatedBy   = createdBy
        };
        _db.Subjects.Add(entity);
        await _db.SaveChangesAsync();

        var instituteNames = await GetInstituteNamesAsync(new[] { entity.InstituteId });
        return MapToDto(entity, instituteNames);
    }

    public async Task<bool> UpdateAsync(int id, UpdateSubjectDto dto, int updatedBy)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(s => s.SubjectId == id);
        if (entity is null) return false;

        var clash = await _db.Subjects.AnyAsync(s => s.SubjectName == dto.SubjectName && s.SubjectId != id);
        if (clash)
            throw new InvalidOperationException($"Another subject already uses the name '{dto.SubjectName}'.");

        entity.SubjectName = dto.SubjectName;
        entity.IsActive    = dto.IsActive;
        entity.UpdatedBy   = updatedBy;
        entity.UpdatedAt   = DateTime.UtcNow;

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
        var entity = await _db.Subjects.FirstOrDefaultAsync(s => s.SubjectId == id);
        if (entity is null) return false;

        var inUse = await _db.ClassSubjects.AnyAsync(cs => cs.SubjectId == id);
        if (inUse)
            throw new InvalidOperationException("Cannot delete a subject that is assigned to one or more classes.");

        entity.IsDeleted = true;
        entity.IsActive  = false;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
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

    private static SubjectDto MapToDto(Subject s, Dictionary<int, string> instituteNames) => new()
    {
        SubjectId     = s.SubjectId,
        SubjectName   = s.SubjectName,
        IsActive      = s.IsActive,
        InstituteId   = s.InstituteId,
        InstituteName = s.InstituteId.HasValue && instituteNames.TryGetValue(s.InstituteId.Value, out var n) ? n : null
    };
}
