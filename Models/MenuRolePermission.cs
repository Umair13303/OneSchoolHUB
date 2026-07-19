namespace SchoolManagement.API.Models;

public class MenuRolePermission
{
    public int Id { get; set; }
    public int MenuItemId { get; set; }
    public int RoleId { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
