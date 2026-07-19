using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.User;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IUserService
{
    Task<List<UserListDto>> GetAllAsync();
    Task<UserListDto?> GetByIdAsync(int id);
    Task<UserListDto> CreateAsync(CreateUserDto dto, int createdBy);
    Task<bool> UpdateAsync(int id, UpdateUserDto dto, int updatedBy);
    Task<bool> DeleteAsync(int id);
}

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db) => _db = db;

    private static UserListDto MapDto(User u) => new()
    {
        UserId        = u.UserId,
        FullName      = u.FullName,
        Email         = u.Email,
        RoleName      = u.Role.RoleName,
        RoleId        = u.RoleId,
        IsActive      = u.IsActive,
        CreatedAt     = u.CreatedAt,
        Password      = u.Password,
        Phone         = u.Phone,
        CNIC          = u.CNIC,
        Gender        = u.Gender,
        Address       = u.Address,
        Qualification = u.Qualification,
        Specialization= u.Specialization,
        DateOfBirth   = u.DateOfBirth,
        JoiningDate   = u.JoiningDate,
        SignatureUrl  = u.SignatureUrl,
    };

    public async Task<List<UserListDto>> GetAllAsync()
    {
        var users = await _db.Users.Include(u => u.Role).ToListAsync();
        return users.Select(MapDto).ToList();
    }

    public async Task<UserListDto?> GetByIdAsync(int id)
    {
        var u = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == id);
        return u is null ? null : MapDto(u);
    }

    public async Task<UserListDto> CreateAsync(CreateUserDto dto, int createdBy)
    {
        var emailExists = await _db.Users.AnyAsync(u => u.Email == dto.Email && !u.IsDeleted);
        if (emailExists) throw new InvalidOperationException($"A user with email '{dto.Email}' already exists.");

        var user = new User
        {
            FullName      = dto.FullName,
            Email         = dto.Email,
            Password      = dto.Password,
            PasswordHash  = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId        = dto.RoleId,
            IsActive      = true,
            CreatedBy     = createdBy,
            Phone         = dto.Phone,
            CNIC          = dto.CNIC,
            Gender        = dto.Gender,
            Address       = dto.Address,
            Qualification = dto.Qualification,
            Specialization= dto.Specialization,
            DateOfBirth   = dto.DateOfBirth,
            JoiningDate   = dto.JoiningDate,
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return (await GetByIdAsync(user.UserId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateUserDto dto, int updatedBy)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return false;

        user.FullName      = dto.FullName;
        user.RoleId        = dto.RoleId;
        user.IsActive      = dto.IsActive;
        user.Phone         = dto.Phone;
        user.CNIC          = dto.CNIC;
        user.Gender        = dto.Gender;
        user.Address       = dto.Address;
        user.Qualification = dto.Qualification;
        user.Specialization= dto.Specialization;
        user.DateOfBirth   = dto.DateOfBirth;
        user.JoiningDate   = dto.JoiningDate;
        user.UpdatedBy     = updatedBy;
        user.UpdatedAt     = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
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
