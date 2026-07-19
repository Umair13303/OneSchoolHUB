using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Academics;

public class UpdateSubjectDto
{
    [Required, StringLength(150)]
    public string SubjectName { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    /// <summary>Superadmin-only: reassign this subject to a different institute. Ignored for institute-scoped admins.</summary>
    public int? InstituteId { get; set; }
}
