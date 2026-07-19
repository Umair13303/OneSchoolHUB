using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.User;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public UserController(IUserService userService, AppDbContext db, IWebHostEnvironment env)
    {
        _userService = userService;
        _db = db;
        _env = env;
    }

    /// <summary>Get all users (SuperAdmin, Admin only)</summary>
    [HttpGet]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    /// <summary>Get user by ID</summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>Create a new user (SuperAdmin, Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        try
        {
            var createdBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var user = await _userService.CreateAsync(dto, createdBy);
            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, user);
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { error = ex.Message }); }
        catch (Exception ex)                 { return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message }); }
    }

    /// <summary>Update user</summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        try
        {
            var updatedBy = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var success = await _userService.UpdateAsync(id, dto, updatedBy);
            if (!success) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex) { return Conflict(new { error = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { error = ex.Message }); }
        catch (Exception ex)                 { return BadRequest(new { error = ex.InnerException?.Message ?? ex.Message }); }
    }

    /// <summary>Soft delete user (SuperAdmin only)</summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "superadmin")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userService.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    /// <summary>Get logged-in user profile</summary>
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();
        return Ok(user);
    }

    /// <summary>Upload signature image for a user</summary>
    [HttpPost("{id}/signature")]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> UploadSignature(int id, IFormFile file)
    {
        var user = await _db.Users.FindAsync(id);
        if (user == null) return NotFound();

        var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "signatures");
        Directory.CreateDirectory(uploads);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"sig_{id}_{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(uploads, fileName);

        using var stream = System.IO.File.Create(fullPath);
        await file.CopyToAsync(stream);

        user.SignatureUrl = $"/uploads/signatures/{fileName}";
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { signatureUrl = user.SignatureUrl });
    }
}
