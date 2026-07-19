using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.ActivityLog;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/activity-logs")]
[Authorize(Roles = "superadmin")]
[Produces("application/json")]
public class ActivityLogController : ControllerBase
{
    private readonly IActivityLogService _service;
    public ActivityLogController(IActivityLogService service) => _service = service;

    /// <summary>
    /// Get paginated activity logs. Superadmin only.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLogs([FromQuery] ActivityLogQuery query)
        => Ok(await _service.GetLogsAsync(query));
}
