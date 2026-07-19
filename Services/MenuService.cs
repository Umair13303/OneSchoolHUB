using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Menu;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IMenuService
{
    /// <summary>Returns the nested menu tree visible to the given role, filtered by institute modules.</summary>
    Task<List<MenuItemTreeDto>> GetMenuForRoleAsync(int roleId);

    /// <summary>Flat list of all menu items + their assigned role IDs (SuperAdmin only).</summary>
    Task<List<MenuItemAdminDto>> GetAllForAdminAsync();

    Task<MenuItemAdminDto>      CreateAsync(CreateMenuItemDto dto, int createdBy);
    Task<bool>                  UpdateAsync(int id, UpdateMenuItemDto dto, int updatedBy);
    Task<bool>                  SoftDeleteAsync(int id, int deletedBy);
    Task<bool>                  AssignRolesAsync(int id, List<int> roleIds);
}

public class MenuService : IMenuService
{
    private readonly AppDbContext _db;
    private readonly ITenantContext _tenant;

    // Route URLs that belong to each module — used to hide menu items when
    // the institute has that module disabled.
    private static readonly Dictionary<string, string[]> ModuleRoutes = new()
    {
        ["moduleAttendance"] = ["/attendance"],
        ["moduleFees"]       = ["/fees"],
        ["moduleHomework"]   = ["/homework"],
        ["moduleExams"]      = ["/exams"],
        ["moduleTimetable"]  = ["/timetable"],
        ["moduleHR"]         = ["/hr", "/staff"],
        ["moduleReports"]    = ["/reports"],
    };

    public MenuService(AppDbContext db, ITenantContext tenant)
    {
        _db     = db;
        _tenant = tenant;
    }

    // ──────────────────────────────────────────────────────────────────────
    // Read — filtered tree for one role
    // ──────────────────────────────────────────────────────────────────────
    public async Task<List<MenuItemTreeDto>> GetMenuForRoleAsync(int roleId)
    {
        var visibleItems = await _db.MenuItems
            .AsNoTracking()
            .Where(m => m.IsActive
                     && m.MenuRolePermissions.Any(p => p.RoleId == roleId))
            .OrderBy(m => m.SortOrder)
            .ThenBy(m => m.Title)
            .Select(m => new MenuItemTreeDto
            {
                MenuItemId = m.MenuItemId,
                ParentId   = m.ParentId,
                Title      = m.Title,
                Icon       = m.Icon,
                RouteUrl   = m.RouteUrl,
                SortOrder  = m.SortOrder
            })
            .ToListAsync();

        // For institute users, remove menu items for disabled modules.
        if (!_tenant.IsSuperAdmin && _tenant.InstituteId.HasValue)
        {
            var institute = await _db.Institutes
                .AsNoTracking()
                .Where(i => i.InstituteId == _tenant.InstituteId.Value)
                .Select(i => new {
                    i.ModuleAttendance, i.ModuleFees, i.ModuleHomework,
                    i.ModuleExams, i.ModuleTimetable, i.ModuleHR, i.ModuleReports
                })
                .FirstOrDefaultAsync();

            if (institute != null)
            {
                var disabledRoutes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (!institute.ModuleAttendance) foreach (var r in ModuleRoutes["moduleAttendance"]) disabledRoutes.Add(r);
                if (!institute.ModuleFees)       foreach (var r in ModuleRoutes["moduleFees"])       disabledRoutes.Add(r);
                if (!institute.ModuleHomework)   foreach (var r in ModuleRoutes["moduleHomework"])   disabledRoutes.Add(r);
                if (!institute.ModuleExams)      foreach (var r in ModuleRoutes["moduleExams"])      disabledRoutes.Add(r);
                if (!institute.ModuleTimetable)  foreach (var r in ModuleRoutes["moduleTimetable"])  disabledRoutes.Add(r);
                if (!institute.ModuleHR)         foreach (var r in ModuleRoutes["moduleHR"])         disabledRoutes.Add(r);
                if (!institute.ModuleReports)    foreach (var r in ModuleRoutes["moduleReports"])    disabledRoutes.Add(r);

                if (disabledRoutes.Count > 0)
                    visibleItems = visibleItems
                        .Where(m => m.RouteUrl == null ||
                                    !disabledRoutes.Any(d => m.RouteUrl.StartsWith(d, StringComparison.OrdinalIgnoreCase)))
                        .ToList();
            }
        }

        // Section headers (RouteUrl == null) have no route of their own — if
        // every child under one was just filtered out (module disabled, or the
        // role simply can't see any of them), the header would otherwise reach
        // the sidebar as a dead, unclickable link. Drop those too.
        var idsWithChildren = visibleItems
            .Where(m => m.ParentId.HasValue)
            .Select(m => m.ParentId!.Value)
            .ToHashSet();
        visibleItems = visibleItems
            .Where(m => m.RouteUrl != null || idsWithChildren.Contains(m.MenuItemId))
            .ToList();

        return BuildTree(visibleItems);
    }

    /// <summary>
    /// Builds the parent → children tree from a flat list. If a child's parent
    /// is not in the visible set (e.g. the role can see the child but not the
    /// section header), the child is promoted to a root so it isn't lost.
    /// </summary>
    private static List<MenuItemTreeDto> BuildTree(List<MenuItemTreeDto> items)
    {
        var byId = items.ToDictionary(i => i.MenuItemId);
        var roots = new List<MenuItemTreeDto>();

        foreach (var item in items)
        {
            if (item.ParentId.HasValue && byId.TryGetValue(item.ParentId.Value, out var parent))
                parent.Children.Add(item);
            else
                roots.Add(item);
        }

        // Sort children by SortOrder for stable rendering.
        foreach (var node in items)
            node.Children = node.Children.OrderBy(c => c.SortOrder).ThenBy(c => c.Title).ToList();

        return roots.OrderBy(r => r.SortOrder).ThenBy(r => r.Title).ToList();
    }

    // ──────────────────────────────────────────────────────────────────────
    // Management (SuperAdmin only)
    // ──────────────────────────────────────────────────────────────────────
    public async Task<List<MenuItemAdminDto>> GetAllForAdminAsync()
    {
        return await _db.MenuItems
            .AsNoTracking()
            .OrderBy(m => m.SortOrder).ThenBy(m => m.Title)
            .Select(m => new MenuItemAdminDto
            {
                MenuItemId = m.MenuItemId,
                ParentId   = m.ParentId,
                Title      = m.Title,
                Icon       = m.Icon,
                RouteUrl   = m.RouteUrl,
                SortOrder  = m.SortOrder,
                IsActive   = m.IsActive,
                RoleIds    = m.MenuRolePermissions.Select(p => p.RoleId).ToList()
            })
            .ToListAsync();
    }

    public async Task<MenuItemAdminDto> CreateAsync(CreateMenuItemDto dto, int createdBy)
    {
        if (dto.ParentId.HasValue)
        {
            var parentExists = await _db.MenuItems.AnyAsync(m => m.MenuItemId == dto.ParentId.Value);
            if (!parentExists)
                throw new ArgumentException($"ParentId {dto.ParentId} does not exist.");
        }

        var entity = new MenuItem
        {
            ParentId  = dto.ParentId,
            Title     = dto.Title,
            Icon      = dto.Icon,
            RouteUrl  = dto.RouteUrl,
            SortOrder = dto.SortOrder,
            IsActive  = dto.IsActive,
            CreatedBy = createdBy
        };

        _db.MenuItems.Add(entity);
        await _db.SaveChangesAsync();

        if (dto.RoleIds.Count > 0)
            await ReplaceRolePermissionsAsync(entity.MenuItemId, dto.RoleIds);

        return (await GetByIdAsync(entity.MenuItemId))!;
    }

    public async Task<bool> UpdateAsync(int id, UpdateMenuItemDto dto, int updatedBy)
    {
        var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.MenuItemId == id);
        if (item is null) return false;

        if (dto.ParentId == id)
            throw new ArgumentException("A menu item cannot be its own parent.");

        if (dto.ParentId.HasValue)
        {
            var parentExists = await _db.MenuItems.AnyAsync(m => m.MenuItemId == dto.ParentId.Value);
            if (!parentExists) throw new ArgumentException($"ParentId {dto.ParentId} does not exist.");
        }

        item.ParentId  = dto.ParentId;
        item.Title     = dto.Title;
        item.Icon      = dto.Icon;
        item.RouteUrl  = dto.RouteUrl;
        item.SortOrder = dto.SortOrder;
        item.IsActive  = dto.IsActive;
        item.UpdatedBy = updatedBy;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id, int deletedBy)
    {
        var item = await _db.MenuItems.FirstOrDefaultAsync(m => m.MenuItemId == id);
        if (item is null) return false;

        // Refuse to delete a section header that still has children, to avoid
        // orphaning entries the user can't reach in the admin UI.
        var hasChildren = await _db.MenuItems.AnyAsync(m => m.ParentId == id && !m.IsDeleted);
        if (hasChildren)
            throw new InvalidOperationException("Cannot delete a menu item that has children. Delete or re-parent them first.");

        item.IsDeleted = true;
        item.IsActive  = false;
        item.UpdatedBy = deletedBy;
        item.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignRolesAsync(int id, List<int> roleIds)
    {
        var exists = await _db.MenuItems.AnyAsync(m => m.MenuItemId == id);
        if (!exists) return false;

        await ReplaceRolePermissionsAsync(id, roleIds);
        return true;
    }

    // ──────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────
    private async Task ReplaceRolePermissionsAsync(int menuItemId, List<int> roleIds)
    {
        var existing = _db.MenuRolePermissions.Where(p => p.MenuItemId == menuItemId);
        _db.MenuRolePermissions.RemoveRange(existing);

        // Deduplicate to satisfy the unique (MenuItemId, RoleId) index.
        foreach (var rid in roleIds.Distinct())
            _db.MenuRolePermissions.Add(new MenuRolePermission { MenuItemId = menuItemId, RoleId = rid });

        await _db.SaveChangesAsync();
    }

    private async Task<MenuItemAdminDto?> GetByIdAsync(int id)
    {
        return await _db.MenuItems
            .AsNoTracking()
            .Where(m => m.MenuItemId == id)
            .Select(m => new MenuItemAdminDto
            {
                MenuItemId = m.MenuItemId,
                ParentId   = m.ParentId,
                Title      = m.Title,
                Icon       = m.Icon,
                RouteUrl   = m.RouteUrl,
                SortOrder  = m.SortOrder,
                IsActive   = m.IsActive,
                RoleIds    = m.MenuRolePermissions.Select(p => p.RoleId).ToList()
            })
            .FirstOrDefaultAsync();
    }
}
