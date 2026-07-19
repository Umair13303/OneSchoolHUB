using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Menu;

/// <summary>Body for <c>POST /api/menu</c> (SuperAdmin only).</summary>
public class CreateMenuItemDto
{
    public int? ParentId { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Icon { get; set; }

    [StringLength(250)]
    public string? RouteUrl { get; set; }

    public int  SortOrder { get; set; } = 0;
    public bool IsActive  { get; set; } = true;

    /// <summary>Role IDs that may see this menu item. Empty list means no one can see it (so it's effectively hidden).</summary>
    public List<int> RoleIds { get; set; } = new();
}
