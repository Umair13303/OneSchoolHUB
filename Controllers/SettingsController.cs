using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Settings;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize]
[Produces("application/json")]
public class SettingsController : ControllerBase
{
    private readonly ISettingsService _svc;
    public SettingsController(ISettingsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _svc.GetAsync());

    [HttpPut]
    [Authorize(Roles = "superadmin,admin")]
    public async Task<IActionResult> Save([FromBody] UpdateSchoolSettingsDto dto)
    {
        try
        {
            var result = await _svc.SaveAsync(dto, CurrentUserId());
            return Ok(result);
        }
        catch (FormatException ex) { return BadRequest(new { error = $"Invalid time format: {ex.Message}" }); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    private int CurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
