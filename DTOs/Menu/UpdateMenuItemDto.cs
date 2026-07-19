using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Menu;

/// <summary>Body for <c>PUT /api/menu/{id}</c> (SuperAdmin only).</summary>
public class UpdateMenuItemDto
{
    public int? ParentId { get; set; }

    [Required, StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Icon { get; set; }

    [StringLength(250)]
    public string? RouteUrl { get; set; }

    public int  SortOrder { get; set; }
    public bool IsActive  { get; set; }
}
