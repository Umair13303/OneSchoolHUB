using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Academics;

public class UpdateClassDto
{
    [Required, StringLength(100)]
    public string ClassName { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Section { get; set; }

    public bool IsActive { get; set; }

    /// <summary>Superadmin-only: reassign this class to a different institute. Ignored for institute-scoped admins.</summary>
    public int? InstituteId { get; set; }
}
