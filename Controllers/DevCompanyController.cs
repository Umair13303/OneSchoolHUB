using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/dev-company")]
public class DevCompanyController : ControllerBase
{
    private readonly AppDbContext _db;
    public DevCompanyController(AppDbContext db) => _db = db;

    /// <summary>Get Dev Company info (public — used by login page branding)</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get()
    {
        var company = await _db.DevCompany.AsNoTracking().FirstOrDefaultAsync();
        if (company == null) return Ok(new DevCompany());
        return Ok(company);
    }

    /// <summary>Update Dev Company info (superadmin only)</summary>
    [HttpPut]
    [Authorize(Roles = "superadmin")]
    public async Task<IActionResult> Update([FromBody] DevCompany dto)
    {
        var company = await _db.DevCompany.FirstOrDefaultAsync();
        if (company == null)
        {
            dto.Id = 1;
            dto.UpdatedAt = DateTime.UtcNow;
            _db.DevCompany.Add(dto);
        }
        else
        {
            company.Name          = dto.Name;
            company.Tagline       = dto.Tagline;
            company.LogoUrl       = dto.LogoUrl;
            company.CopyrightText = dto.CopyrightText;
            company.Address       = dto.Address;
            company.Phone         = dto.Phone;
            company.Email         = dto.Email;
            company.Website       = dto.Website;
            company.UpdatedAt     = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return Ok(company ?? dto);
    }
}
