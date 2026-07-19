namespace SchoolManagement.API.Models;

public class Campus : BaseEntity
{
    public new int CampusId { get; set; }  // PK, overrides BaseEntity.CampusId
    public new int InstituteId { get; set; }  // non-nullable FK, overrides BaseEntity.InstituteId
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    public Institute Institute { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}
