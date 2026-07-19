namespace SchoolManagement.API.DTOs.Menu;

/// <summary>
/// Single node in the menu tree returned by <c>GET /api/menu</c>.
/// Children are nested so the Angular sidebar can render the tree directly.
/// </summary>
public class MenuItemTreeDto
{
    public int    MenuItemId { get; set; }
    public int?   ParentId   { get; set; }
    public string Title      { get; set; } = string.Empty;
    public string? Icon      { get; set; }
    public string? RouteUrl  { get; set; }
    public int    SortOrder  { get; set; }

    public List<MenuItemTreeDto> Children { get; set; } = new();
}
