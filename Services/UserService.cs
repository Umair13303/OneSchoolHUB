using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.User;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IUserService
{
    Task<List<UserListDto>> GetAllAsync();
    Task<UserListDto?> GetByIdAsync(int id);
    Task<UserListDto?> GetProfileAsync(int id);
    Task<UserListDto> CreateAsync(CreateUserDto dto, int createdBy);
    Task<bool> UpdateAsync(int id, UpdateUserDto dto, int updatedBy);
    Task<bool> DeleteAsync(int id);
    Task<UserListDto?> UpdateMyProfileAsync(int userId, UpdateMyProfileDto dto);
    Task ChangeMyPasswordAsync(int userId, ChangeMyPasswordDto dto);
    Task<UserListDto?> UpdateMyPhotoAsync(int userId, int photoFileId);
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    private static UserListDto MapDto(User u, bool includePassword = false) => new()
    {
        UserId         = u.UserId,
        FullName       = u.FullName,
        Email          = u.Email,
        RoleName       = u.Role.RoleName,
        RoleId         = u.RoleId,
        IsActive       = u.IsActive,
        CreatedAt      = u.CreatedAt,
        Password       = includePassword ? u.Password : null,
        Phone          = u.Phone,
        CNIC           = u.CNIC,
        Gender         = u.Gender,
        Address        = u.Address,
        Qualification  = u.Qualification,
        Specialization = u.Specialization,
        DateOfBirth    = u.DateOfBirth,
        JoiningDate    = u.JoiningDate,
        SignatureUrl   = u.SignatureUrl,
        PhotoFileId    = u.PhotoFileId,
    };

    public async Task<List<UserListDto>> GetAllAsync()
    {
        var users = await _db.Users.Include(u => u.Role).ToListAsync();
        return users.Select(u => MapDto(u, includePassword: true)).ToList();
    }

    public async Task<UserListDto?> GetByIdAsync(int id)
    {
        var u = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
        return u is null ? null : MapDto(u, includePassword: true);
    }

    public async Task<UserListDto?> GetProfileAsync(int id)
    {
        var u = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
        return u is null ? null : MapDto(u, includePassword: false);
    }

    public async Task<UserListDto> CreateAsync(CreateUserDto dto, int createdBy)
    {
        var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted);
        if (emailExists) throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");

        var user = new User
        {
            FullName       = dto.FullName,
            Email          = dto.Email,
            Password       = dto.Password,
            PasswordHash   = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId         = dto.RoleId,
            IsActive       = true,
            CreatedBy      = createdBy,
            Phone          = dto.Phone,
            CNIC           = dto.CNIC,
            Gender         = dto.Gender,
            Address        = dto.Address,
            Qualification  = dto.Qualification,
            Specialization = dto.Specialization,
            DateOfBirth    = dto.DateOfBirth,
            JoiningDate    = dto.JoiningDate,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(user.UserId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto, int updatedBy)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return false;

        user.FullName       = dto.FullName;
        user.RoleId         = dto.RoleId;
        user.IsActive       = dto.IsActive;
        user.Phone          = dto.Phone;
        user.CNIC           = dto.CNIC;
        user.Gender         = dto.Gender;
        user.Address        = dto.Address;
        user.Qualification  = dto.Qualification;
        user.Specialization = dto.Specialization;
        user.DateOfBirth    = dto.DateOfBirth;
        user.JoiningDate    = dto.JoiningDate;
        user.UpdatedBy      = updatedBy;
        user.UpdatedAt      = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<UserListDto?> UpdateMyProfileAsync(int userId, UpdateMyProfileDto dto)
    {
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return null;

        var fullName = (dto.FullName ?? "").Trim();
        var email = (dto.Email ?? "").Trim();
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.");

        var emailTaken = await _db.Users.AnyAsync(u =>
            u.Email == email && u.UserId != userId && !u.IsDeleted);
        if (emailTaken)
            throw new InvalidOperationException($"A user with email '{email}' already exists.");

        user.FullName = fullName;
        user.Email = email;
        user.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        user.Gender = string.IsNullOrWhiteSpace(dto.Gender) ? null : dto.Gender.Trim();
        user.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
        user.DateOfBirth = dto.DateOfBirth;
        user.CNIC = string.IsNullOrWhiteSpace(dto.CNIC) ? null : dto.CNIC.Trim();
        user.Qualification = string.IsNullOrWhiteSpace(dto.Qualification) ? null : dto.Qualification.Trim();
        user.Specialization = string.IsNullOrWhiteSpace(dto.Specialization) ? null : dto.Specialization.Trim();
        user.UpdatedBy = userId;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapDto(user, includePassword: false);
    }

    public async Task ChangeMyPasswordAsync(int userId, ChangeMyPasswordDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId)
            ?? throw new InvalidOperationException("User not found.");

        if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
            throw new ArgumentException("Current and new password are required.");
        if (dto.NewPassword.Trim().Length < 6)
            throw new ArgumentException("New password must be at least 6 characters.");
        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect.");

        var next = dto.NewPassword.Trim();
        user.Password = next;
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(next);
        user.UpdatedBy = userId;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task<UserListDto?> UpdateMyPhotoAsync(int userId, int photoFileId)
    {
        var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return null;

        var file = await _db.FileStores.FirstOrDefaultAsync(f => f.FileId == photoFileId && !f.IsDeleted)
            ?? throw new ArgumentException("Photo file not found.");

        file.EntityType = "user-photo";
        file.EntityId = userId;
        user.PhotoFileId = photoFileId;
        user.UpdatedBy = userId;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapDto(user, includePassword: false);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return false;

        user.IsDeleted = true;
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }
}
