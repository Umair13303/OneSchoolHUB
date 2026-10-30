using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/my-institute")]
[Authorize]
public class MyInstituteController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITenantContext _tenant;
    private readonly IInstituteMembershipService _membership;

    public MyInstituteController(
        AppDbContext db,
        ITenantContext tenant,
        IInstituteMembershipService membership)
    {
        _db = db;
        _tenant = tenant;
        _membership = membership;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int? instituteId = null)
    {
        // Resolve which institute to use:
        // - non-superadmin: JWT tenant, then heal/infer from user membership
        // - superadmin: optional ?instituteId (challan printing context)
        int? resolvedId = _tenant.IsSuperAdmin ? instituteId : _tenant.InstituteId;

        if (!resolvedId.HasValue && !_tenant.IsSuperAdmin)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (int.TryParse(userIdClaim, out var userId))
            {
                var role = User.FindFirstValue(ClaimTypes.Role) ?? "";
                var row = await _db.Users.AsNoTracking().IgnoreQueryFilters()
                    .Where(u => u.UserId == userId && !u.IsDeleted)
                    .Select(u => new { u.InstituteId, u.CampusId })
                    .FirstOrDefaultAsync();

                var membership = await _membership.ResolveAsync(
                    userId, row?.InstituteId, row?.CampusId, role, heal: true);
                resolvedId = membership?.InstituteId;
            }
        }

        if (!resolvedId.HasValue)
            return NoContent();

        var i = await _db.Institutes
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.InstituteId == resolvedId);

        if (i == null) return NoContent();

        return Ok(new { i.InstituteId, i.Name, i.LogoUrl, i.ChallanTemplate, i.SchoolStampUrl });
    }
}
