namespace SchoolManagement.API.Models;

public class MenuItem : BaseEntity
{
    public int MenuItemId { get; set; }
    public int? ParentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? RouteUrl { get; set; }
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    public MenuItem? Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();
    public ICollection<MenuRolePermission> MenuRolePermissions { get; set; } = new List<MenuRolePermission>();
}
