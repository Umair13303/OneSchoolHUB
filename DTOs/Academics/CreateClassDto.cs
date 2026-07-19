using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Academics;

public class CreateClassDto
{
    [Required, StringLength(100)]
    public string ClassName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Section { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>Required when the caller is superadmin — which institute this class belongs to. Ignored for institute-scoped admins (their own institute is stamped automatically).</summary>
    public int? InstituteId { get; set; }
}
