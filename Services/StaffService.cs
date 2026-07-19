using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Staff;
using SchoolManagement.API.Helpers;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IStaffService
{
    Task<List<StaffDto>> GetAllAsync(string? search, string? department, string? status);
    Task<StaffDto?> GetByIdAsync(int id);
    Task<StaffDto> CreateAsync(CreateStaffDto dto, int createdBy);
    Task<bool> UpdateAsync(int id, UpdateStaffDto dto, int updatedBy);
    Task<bool> DeleteAsync(int id, int deletedBy);
    Task<StaffDto> CreateLoginAsync(int staffId, CreateStaffLoginDto dto, int createdBy);
    Task<List<StaffDocumentDto>> GetDocumentsAsync(int staffId);
    Task<StaffDocumentDto?> TagDocumentAsync(int staffId, TagStaffDocumentDto dto);
    Task<bool> RemoveDocumentAsync(int staffId, int fileId);
}

public class StaffService : IStaffService
{
    private readonly AppDbContext _db;
    private readonly JwtHelper _jwt;
    private readonly IConfiguration _config;

    public StaffService(AppDbContext db, JwtHelper jwt, IConfiguration config)
    {
        _db     = db;
        _jwt    = jwt;
        _config = config;
    }

    public async Task<List<StaffDto>> GetAllAsync(string? search, string? department, string? status)
    {
        var q = _db.Staff.AsNoTracking()
            .Where(s => !s.IsDeleted)
            .Include(s => s.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var t = search.Trim();
            q = q.Where(s => s.FullName.Contains(t) || (s.Cnic != null && s.Cnic.Contains(t)) || (s.Phone != null && s.Phone.Contains(t)));
        }
        if (!string.IsNullOrWhiteSpace(department))
            q = q.Where(s => s.Department == department);
        if (!string.IsNullOrWhiteSpace(status))
            q = q.Where(s => s.Status == status);

        return await q.OrderBy(s => s.FullName).Select(s => Map(s)).ToListAsync();
    }

    public async Task<StaffDto?> GetByIdAsync(int id)
    {
        var s = await _db.Staff.AsNoTracking()
            .Where(s => s.StaffId == id && !s.IsDeleted)
            .Include(s => s.User)
            .FirstOrDefaultAsync();
        return s is null ? null : Map(s);
    }

    public async Task<StaffDto> CreateAsync(CreateStaffDto dto, int createdBy)
    {
        var entity = new Staff
        {
            FullName             = dto.FullName.Trim(),
            Cnic                 = dto.Cnic?.Trim(),
            Gender               = dto.Gender,
            DateOfBirth          = dto.DateOfBirth,
            Phone                = dto.Phone?.Trim(),
            Address              = dto.Address?.Trim(),
            EmergencyContactName = dto.EmergencyContactName?.Trim(),
            EmergencyContactPhone= dto.EmergencyContactPhone?.Trim(),
            Designation          = dto.Designation.Trim(),
            Department           = dto.Department?.Trim(),
            JoiningDate          = dto.JoiningDate,
            EmploymentType       = dto.EmploymentType,
            Status               = dto.Status,
            PhotoFileId          = dto.PhotoFileId,
            CreatedBy            = createdBy
        };
        _db.Staff.Add(entity);
        await _db.SaveChangesAsync();
        return (await GetByIdAsync(entity.StaffId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateStaffDto dto, int updatedBy)
    {
        var entity = await _db.Staff.FirstOrDefaultAsync(s => s.StaffId == id && !s.IsDeleted);
        if (entity is null) return false;

        entity.FullName              = dto.FullName.Trim();
        entity.Cnic                  = dto.Cnic?.Trim();
        entity.Gender                = dto.Gender;
        entity.DateOfBirth           = dto.DateOfBirth;
        entity.Phone                 = dto.Phone?.Trim();
        entity.Address               = dto.Address?.Trim();
        entity.EmergencyContactName  = dto.EmergencyContactName?.Trim();
        entity.EmergencyContactPhone = dto.EmergencyContactPhone?.Trim();
        entity.Designation           = dto.Designation.Trim();
        entity.Department            = dto.Department?.Trim();
        entity.JoiningDate           = dto.JoiningDate;
        entity.EmploymentType        = dto.EmploymentType;
        entity.Status                = dto.Status;
        entity.PhotoFileId           = dto.PhotoFileId;
        entity.UpdatedBy             = updatedBy;
        entity.UpdatedAt             = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int deletedBy)
    {
        var entity = await _db.Staff.FirstOrDefaultAsync(s => s.StaffId == id && !s.IsDeleted);
        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<StaffDto> CreateLoginAsync(int staffId, CreateStaffLoginDto dto, int createdBy)
    {
        var staff = await _db.Staff.FirstOrDefaultAsync(s => s.StaffId == staffId && !s.IsDeleted)
            ?? throw new ArgumentException("Staff not found.");

        if (staff.UserId.HasValue)
            throw new InvalidOperationException("This staff member already has a login.");

        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already in use.");

        var staffRole = await _db.Roles.FirstAsync(r => r.RoleName == "staff");
        var user = new User
        {
            FullName     = staff.FullName,
            Email        = dto.Email.Trim().ToLower(),
            Password     = dto.Password,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId       = staffRole.RoleId,
            IsActive     = true,
            CreatedBy    = createdBy
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        staff.UserId    = user.UserId;
        staff.UpdatedBy = createdBy;
        staff.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(staffId))!;
    }

    // ── Documents (degree certs, CNIC, etc — open-ended, admin-labeled) ────
    public async Task<List<StaffDocumentDto>> GetDocumentsAsync(int staffId)
    {
        var fileServerBaseUrl = (_config["FileServer:BaseUrl"] ?? string.Empty).TrimEnd('/');

        return await _db.FileStores
            .AsNoTracking()
            .Where(f => !f.IsDeleted
                     && f.EntityId == staffId
                     && f.EntityType == "staff-document")
            .OrderByDescending(f => f.UploadedAt)
            .Select(f => new StaffDocumentDto
            {
                FileId       = f.FileId,
                Label        = f.Label ?? "Document",
                OriginalName = f.OriginalName,
                FileType     = f.FileType,
                SizeBytes    = f.SizeBytes,
                UploadedAt   = f.UploadedAt,
                FileUrl      = fileServerBaseUrl + "/api/files/" + f.FileId
            })
            .ToListAsync();
    }

    public async Task<StaffDocumentDto?> TagDocumentAsync(int staffId, TagStaffDocumentDto dto)
    {
        var staffExists = await _db.Staff.AnyAsync(s => s.StaffId == staffId && !s.IsDeleted);
        if (!staffExists) throw new ArgumentException("Staff not found.");

        var file = await _db.FileStores.FirstOrDefaultAsync(f => f.FileId == dto.FileId && !f.IsDeleted);
        if (file is null) throw new ArgumentException("Uploaded file not found.");

        // A prior document under the same label is superseded by this one.
        var previous = await _db.FileStores.FirstOrDefaultAsync(f =>
            !f.IsDeleted && f.EntityId == staffId && f.EntityType == "staff-document" &&
            f.Label == dto.Label && f.FileId != dto.FileId);
        if (previous is not null) previous.IsDeleted = true;

        file.EntityId   = staffId;
        file.EntityType = "staff-document";
        file.Label      = dto.Label.Trim();
        await _db.SaveChangesAsync();

        var fileServerBaseUrl = (_config["FileServer:BaseUrl"] ?? string.Empty).TrimEnd('/');
        return new StaffDocumentDto
        {
            FileId       = file.FileId,
            Label        = file.Label ?? "Document",
            OriginalName = file.OriginalName,
            FileType     = file.FileType,
            SizeBytes    = file.SizeBytes,
            UploadedAt   = file.UploadedAt,
            FileUrl      = fileServerBaseUrl + "/api/files/" + file.FileId
        };
    }

    public async Task<bool> RemoveDocumentAsync(int staffId, int fileId)
    {
        var file = await _db.FileStores.FirstOrDefaultAsync(f =>
            f.FileId == fileId && f.EntityId == staffId && f.EntityType == "staff-document" && !f.IsDeleted);
        if (file is null) return false;

        file.IsDeleted = true;
        await _db.SaveChangesAsync();
        return true;
    }

    private static StaffDto Map(Staff s) => new()
    {
        StaffId              = s.StaffId,
        FullName             = s.FullName,
        Cnic                 = s.Cnic,
        Gender               = s.Gender,
        DateOfBirth          = s.DateOfBirth,
        Phone                = s.Phone,
        Address              = s.Address,
        EmergencyContactName = s.EmergencyContactName,
        EmergencyContactPhone= s.EmergencyContactPhone,
        Designation          = s.Designation,
        Department           = s.Department,
        JoiningDate          = s.JoiningDate,
        EmploymentType       = s.EmploymentType,
        Status               = s.Status,
        PhotoFileId          = s.PhotoFileId,
        UserId               = s.UserId,
        UserEmail            = s.User?.Email
    };
}
