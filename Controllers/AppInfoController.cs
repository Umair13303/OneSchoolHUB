using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/app-info")]
[AllowAnonymous]
public class AppInfoController : ControllerBase
{
    private readonly AppDbContext _db;
    public AppInfoController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dev = await _db.DevCompany.AsNoTracking().FirstOrDefaultAsync();
        return Ok(new
        {
            appName       = dev?.Name          ?? "Dev_Solutions",
            tagline       = dev?.Tagline        ?? string.Empty,
            logoUrl       = dev?.LogoUrl        ?? string.Empty,
            copyrightText = dev?.CopyrightText  ?? string.Empty
        });
    }
}
