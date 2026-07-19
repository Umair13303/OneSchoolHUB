using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Inventory;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>Inventory & POS configuration (SRS §17).</summary>
[ApiController]
[Route("api/inventory/settings")]
[Authorize]
[Produces("application/json")]
public class InventorySettingsController : ControllerBase
{
    private readonly IInventorySettingsService _service;
    public InventorySettingsController(IInventorySettingsService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _service.GetAsync());

    [HttpPut]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Update([FromBody] UpdateInventorySettingsDto dto)
        => Ok(await _service.UpdateAsync(dto, CurrentUserId()));

    private int CurrentUserId() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
}
