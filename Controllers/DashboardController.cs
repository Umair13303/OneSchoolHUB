using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Lightweight dashboard aggregates for admin/principal (and superadmin).
///   GET /api/dashboard/summary
/// </summary>
[ApiController]
[Route("api/dashboard")]
[Authorize]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service) => _service = service;

    /// <summary>
    /// Student totals, gender breakdown, and class-name enrollment counts for the
    /// admin dashboard widgets. Replaces client-side reduction of the full roster.
    /// </summary>
    [HttpGet("summary")]
    [Authorize(Roles = "superadmin,admin,principal")]
    public async Task<IActionResult> GetSummary()
        => Ok(await _service.GetSummaryAsync());
}
