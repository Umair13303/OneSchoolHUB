namespace SchoolManagement.API.Models;

public abstract class BaseEntity
{
    public bool IsDeleted { get; set; } = false;
    public int? CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Multi-tenant columns — nullable so superadmin/system rows don't require them
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }

}
