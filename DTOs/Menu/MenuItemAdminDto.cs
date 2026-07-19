namespace SchoolManagement.API.DTOs.Menu;

/// <summary>
/// Flat view of a menu item with its assigned role IDs — used by the
/// SuperAdmin-only management screens. Not filtered by role.
/// </summary>
public class MenuItemAdminDto
{
    public int    MenuItemId { get; set; }
    public int?   ParentId   { get; set; }
    public string Title      { get; set; } = string.Empty;
    public string? Icon      { get; set; }
    public string? RouteUrl  { get; set; }
    public int    SortOrder  { get; set; }
    public bool   IsActive   { get; set; }

    /// <summary>IDs of the roles allowed to see this item.</summary>
    public List<int> RoleIds  { get; set; } = new();
}
