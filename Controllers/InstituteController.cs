using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Institute;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/institutes")]
[Authorize(Roles = "superadmin")]
public class InstituteController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly ITenantContext _tenant;

    public InstituteController(AppDbContext db, IWebHostEnvironment env, ITenantContext tenant)
    {
        _db = db;
        _env = env;
        _tenant = tenant;
    }

    // Any authenticated user can call this to get their own institute's name + logo.
    // Superadmin (no institute) gets a 204 No Content so the client can skip gracefully.
    [HttpGet("my")]
    [Authorize(Roles = "superadmin,admin,principal,teacher,staff")]
    public async Task<IActionResult> GetMine()
    {
        if (_tenant.IsSuperAdmin || !_tenant.InstituteId.HasValue)
            return NoContent();

        var i = await _db.Institutes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.InstituteId == _tenant.InstituteId);

        if (i == null) return NoContent();

        return Ok(new { i.InstituteId, i.Name, i.LogoUrl, i.ChallanTemplate, i.SchoolStampUrl });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _db.Institutes
            .Include(i => i.Campuses)
            .OrderBy(i => i.Name)
            .Select(i => new InstituteDto
            {
                InstituteId      = i.InstituteId,
                Name             = i.Name,
                Address          = i.Address,
                Phone            = i.Phone,
                Email            = i.Email,
                Website          = i.Website,
                LogoUrl          = i.LogoUrl,
                IsActive         = i.IsActive,
                LicenseValidUntil = i.LicenseValidUntil,
                ModuleAttendance = i.ModuleAttendance,
                ModuleFees       = i.ModuleFees,
                ModuleHomework   = i.ModuleHomework,
                ModuleExams      = i.ModuleExams,
                ModuleTimetable  = i.ModuleTimetable,
                ModuleHR         = i.ModuleHR,
                ModuleReports    = i.ModuleReports,
                CampusCount      = i.Campuses.Count(c => !c.IsDeleted),
                ChallanTemplate  = i.ChallanTemplate,
                SchoolStampUrl   = i.SchoolStampUrl,
                CreatedAt        = i.CreatedAt
            }).ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id}")]
    [Authorize]  // any authenticated user can fetch their own institute info
    public async Task<IActionResult> GetById(int id)
    {
        var i = await _db.Institutes.Include(x => x.Campuses).FirstOrDefaultAsync(x => x.InstituteId == id);
        if (i == null) return NotFound();

        return Ok(new InstituteDto
        {
            InstituteId      = i.InstituteId,
            Name             = i.Name,
            Address          = i.Address,
            Phone            = i.Phone,
            Email            = i.Email,
            Website          = i.Website,
            LogoUrl          = i.LogoUrl,
            IsActive         = i.IsActive,
            LicenseValidUntil = i.LicenseValidUntil,
            ModuleAttendance = i.ModuleAttendance,
            ModuleFees       = i.ModuleFees,
            ModuleHomework   = i.ModuleHomework,
            ModuleExams      = i.ModuleExams,
            ModuleTimetable  = i.ModuleTimetable,
            ModuleHR         = i.ModuleHR,
            ModuleReports    = i.ModuleReports,
            CampusCount      = i.Campuses.Count(c => !c.IsDeleted),
            ChallanTemplate  = i.ChallanTemplate,
            SchoolStampUrl   = i.SchoolStampUrl,
            CreatedAt        = i.CreatedAt
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInstituteDto dto)
    {
        var institute = new Institute
        {
            Name             = dto.Name,
            Address          = dto.Address,
            Phone            = dto.Phone,
            Email            = dto.Email,
            Website          = dto.Website,
            LicenseValidUntil = dto.LicenseValidUntil,
            ModuleAttendance = dto.ModuleAttendance,
            ModuleFees       = dto.ModuleFees,
            ModuleHomework   = dto.ModuleHomework,
            ModuleExams      = dto.ModuleExams,
            ModuleTimetable  = dto.ModuleTimetable,
            ModuleHR         = dto.ModuleHR,
            ModuleReports    = dto.ModuleReports,
            IsActive         = true
        };
        _db.Institutes.Add(institute);
        await _db.SaveChangesAsync();
        return Ok(new { institute.InstituteId, message = "Institute created." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInstituteDto dto)
    {
        var institute = await _db.Institutes.FindAsync(id);
        if (institute == null) return NotFound();

        institute.Name             = dto.Name;
        institute.Address          = dto.Address;
        institute.Phone            = dto.Phone;
        institute.Email            = dto.Email;
        institute.Website          = dto.Website;
        institute.IsActive         = dto.IsActive;
        institute.LicenseValidUntil = dto.LicenseValidUntil;
        institute.ModuleAttendance = dto.ModuleAttendance;
        institute.ModuleFees       = dto.ModuleFees;
        institute.ModuleHomework   = dto.ModuleHomework;
        institute.ModuleExams      = dto.ModuleExams;
        institute.ModuleTimetable  = dto.ModuleTimetable;
        institute.ModuleHR         = dto.ModuleHR;
        institute.ModuleReports    = dto.ModuleReports;
        institute.UpdatedAt        = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Institute updated." });
    }

    [HttpPatch("{id}/modules")]
    public async Task<IActionResult> UpdateModules(int id, [FromBody] InstituteModulesDto dto)
    {
        var institute = await _db.Institutes.FindAsync(id);
        if (institute == null) return NotFound();

        institute.ModuleAttendance = dto.ModuleAttendance;
        institute.ModuleFees       = dto.ModuleFees;
        institute.ModuleHomework   = dto.ModuleHomework;
        institute.ModuleExams      = dto.ModuleExams;
        institute.ModuleTimetable  = dto.ModuleTimetable;
        institute.ModuleHR         = dto.ModuleHR;
        institute.ModuleReports    = dto.ModuleReports;
        institute.UpdatedAt        = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Module permissions updated." });
    }

    [HttpPatch("{id}/challan-template")]
    public async Task<IActionResult> UpdateChallanTemplate(int id, [FromBody] InstituteChallanTemplateDto dto)
    {
        var institute = await _db.Institutes.FindAsync(id);
        if (institute == null) return NotFound();

        institute.ChallanTemplate = dto.ChallanTemplate;
        institute.UpdatedAt       = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Challan template updated." });
    }

    [HttpPost("{id}/logo")]
    public async Task<IActionResult> UploadLogo(int id, IFormFile file)
    {
        var institute = await _db.Institutes.FindAsync(id);
        if (institute == null) return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
            return BadRequest("Only image files are allowed.");

        var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "logos");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"institute_{id}_{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        institute.LogoUrl   = $"/logos/{fileName}";
        institute.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { logoUrl = institute.LogoUrl });
    }

    [HttpPost("{id}/stamp")]
    public async Task<IActionResult> UploadStamp(int id, IFormFile file)
    {
        var institute = await _db.Institutes.FindAsync(id);
        if (institute == null) return NotFound();

        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var ext = Path.GetExtension(file.FileName).ToLower();
        if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp")
            return BadRequest("Only image files are allowed.");

        var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "stamps");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"stamp_{id}_{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
            await file.CopyToAsync(stream);

        institute.SchoolStampUrl = $"/uploads/stamps/{fileName}";
        institute.UpdatedAt      = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return Ok(new { schoolStampUrl = institute.SchoolStampUrl });
    }

    [HttpPost("{id}/admin")]
    public async Task<IActionResult> CreateAdmin(int id, [FromBody] CreateCampusAdminDto dto)
    {
        var institute = await _db.Institutes.FindAsync(id);
        if (institute == null) return NotFound("Institute not found.");

        if (await _db.Users.IgnoreQueryFilters().AnyAsync(u => u.Email == dto.Email && !u.IsDeleted))
            return BadRequest(new { error = "Email already in use." });

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
            InstituteId  = id,
            CampusId     = dto.CampusId,
            IsActive     = true
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { user.UserId, message = "Institute admin created." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var institute = await _db.Institutes.FindAsync(id);
        if (institute == null) return NotFound();

        institute.IsDeleted = true;
        institute.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Institute deleted." });
    }
}
