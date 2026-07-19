using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Institute;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/institutes/{instituteId}/campuses")]
[Authorize(Roles = "superadmin")]
public class CampusController : ControllerBase
{
    private readonly AppDbContext _db;

    public CampusController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll(int instituteId)
    {
        var campuses = await _db.Campuses
            .Where(c => c.InstituteId == instituteId)
            .Select(c => new CampusDto
            {
                CampusId    = c.CampusId,
                InstituteId = c.InstituteId,
                Name        = c.Name,
                Address     = c.Address,
                Phone       = c.Phone,
                IsActive    = c.IsActive,
                UserCount   = _db.Users.Count(u => u.CampusId == c.CampusId && !u.IsDeleted),
                CreatedAt   = c.CreatedAt
            }).ToListAsync();

        return Ok(campuses);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int instituteId, [FromBody] CreateCampusDto dto)
    {
        var institute = await _db.Institutes.FindAsync(instituteId);
        if (institute == null) return NotFound("Institute not found.");

        var campus = new Campus
        {
            InstituteId = instituteId,
            Name        = dto.Name,
            Address     = dto.Address,
            Phone       = dto.Phone,
            IsActive    = true
        };
        _db.Campuses.Add(campus);
        await _db.SaveChangesAsync();
        return Ok(new { campus.CampusId, message = "Campus created." });
    }

    [HttpPut("{campusId}")]
    public async Task<IActionResult> Update(int instituteId, int campusId, [FromBody] UpdateCampusDto dto)
    {
        var campus = await _db.Campuses.FirstOrDefaultAsync(c => c.CampusId == campusId && c.InstituteId == instituteId);
        if (campus == null) return NotFound();

        campus.Name      = dto.Name;
        campus.Address   = dto.Address;
        campus.Phone     = dto.Phone;
        campus.IsActive  = dto.IsActive;
        campus.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Campus updated." });
    }

    [HttpDelete("{campusId}")]
    public async Task<IActionResult> Delete(int instituteId, int campusId)
    {
        var campus = await _db.Campuses.FirstOrDefaultAsync(c => c.CampusId == campusId && c.InstituteId == instituteId);
        if (campus == null) return NotFound();

        campus.IsDeleted = true;
        campus.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Campus deleted." });
    }

    // Create admin user for this campus
    [HttpPost("{campusId}/admins")]
    public async Task<IActionResult> CreateAdmin(int instituteId, int campusId, [FromBody] CreateCampusAdminDto dto)
    {
        var campus = await _db.Campuses.FirstOrDefaultAsync(c => c.CampusId == campusId && c.InstituteId == instituteId);
        if (campus == null) return NotFound("Campus not found.");

        if (await _db.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
            return BadRequest("Email already in use.");

        var adminRoleId = await _db.Roles
            .Where(r => r.RoleName == "admin")
            .Select(r => r.RoleId)
            .FirstOrDefaultAsync();

        var user = new User
        {
            FullName     = dto.FullName,
            Email        = dto.Email,
            Password     = dto.Password,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId       = adminRoleId,
            InstituteId  = instituteId,
            CampusId     = campusId,
            IsActive     = true
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { user.UserId, message = "Admin user created." });
    }

    // List admin users for this campus
    [HttpGet("{campusId}/admins")]
    public async Task<IActionResult> GetAdmins(int instituteId, int campusId)
    {
        var admins = await _db.Users
            .Include(u => u.Role)
            .Where(u => u.CampusId == campusId && u.InstituteId == instituteId && !u.IsDeleted)
            .Select(u => new
            {
                u.UserId,
                u.FullName,
                u.Email,
                u.IsActive,
                Role = u.Role.RoleName
            }).ToListAsync();

        return Ok(admins);
    }
}
