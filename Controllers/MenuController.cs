using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Menu;
using SchoolManagement.API.Services;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Module 2 — Dynamic Menu. Returns the menu tree filtered for the calling
/// user's role, plus SuperAdmin-only endpoints for managing menu items and
/// their role permissions.
/// </summary>
[ApiController]
[Route("api/menu")]
[Authorize]
[Produces("application/json")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menu;

    public MenuController(IMenuService menu) => _menu = menu;

    /// <summary>
    /// Returns the menu tree the logged-in user is allowed to see. This is the
    /// endpoint the Angular sidebar calls on login.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetForCurrentUser()
    {
        var roleId = GetCurrentRoleId();
        if (roleId is null)
            return Forbid();

        var tree = await _menu.GetMenuForRoleAsync(roleId.Value);
        return Ok(tree);
    }

    // ── Management endpoints (SuperAdmin only) ────────────────────────────
    // Per the task doc's "Module Access by Role" table, only SuperAdmin can
    // touch Menu Management.

    /// <summary>All menu items with assigned role IDs — flat list for the management UI.</summary>
    [HttpGet("all")]
    [Authorize(Roles = "superadmin")]
    public async Task<IActionResult> GetAllForAdmin()
    {
        var items = await _menu.GetAllForAdminAsync();
        return Ok(items);
    }

    /// <summary>Create a new menu item and (optionally) assign roles.</summary>
    [HttpPost]
    [Authorize(Roles = "superadmin")]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto)
    {
        try
        {
            var createdBy = GetCurrentUserId();
            var item = await _menu.CreateAsync(dto, createdBy);
            // No GET-by-id endpoint, so Location points at the management list
            // where the new item will appear.
            return CreatedAtAction(nameof(GetAllForAdmin), null, item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Update menu item properties (does not change role assignments — use /roles for that).</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "superadmin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto dto)
    {
        try
        {
            var updatedBy = GetCurrentUserId();
            var ok = await _menu.UpdateAsync(id, dto, updatedBy);
            return ok ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Soft delete a menu item. Items with children are rejected.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "superadmin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deletedBy = GetCurrentUserId();
            var ok = await _menu.SoftDeleteAsync(id, deletedBy);
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>Replace the role-permissions set for a menu item.</summary>
    [HttpPut("{id:int}/roles")]
    [Authorize(Roles = "superadmin")]
    public async Task<IActionResult> AssignRoles(int id, [FromBody] AssignMenuRolesDto dto)
    {
        var ok = await _menu.AssignRolesAsync(id, dto.RoleIds);
        return ok ? NoContent() : NotFound();
    }

    // ──────────────────────────────────────────────────────────────────────
    private int GetCurrentUserId()
        => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

    private int? GetCurrentRoleId()
    {
        var raw = User.FindFirst("roleId")?.Value;
        return int.TryParse(raw, out var id) ? id : (int?)null;
    }
}
