namespace SchoolManagement.API.DTOs.Menu;

/// <summary>Body for <c>PUT /api/menu/{id}/roles</c> — replaces the menu item's allowed-roles set.</summary>
public class AssignMenuRolesDto
{
    public List<int> RoleIds { get; set; } = new();
}
