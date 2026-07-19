using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Infrastructure;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/my-institute")]
[Authorize]
public class MyInstituteController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITenantContext _tenant;

    public MyInstituteController(AppDbContext db, ITenantContext tenant)
    {
        _db = db;
        _tenant = tenant;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? instituteId = null)
    {
        // Resolve which institute to use:
        // - non-superadmin: always their own institute from the JWT
        // - superadmin: use the optional ?instituteId query param (for challan printing context)
        int? resolvedId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

        if (!resolvedId.HasValue)
            return NoContent();

        var i = await _db.Institutes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.InstituteId == resolvedId);

        if (i == null) return NoContent();

        return Ok(new { i.InstituteId, i.Name, i.LogoUrl, i.ChallanTemplate, i.SchoolStampUrl });
    }
}
