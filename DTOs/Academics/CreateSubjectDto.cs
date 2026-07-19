using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Academics;

public class CreateSubjectDto
{
    [Required, StringLength(150)]
    public string SubjectName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    /// <summary>Required when the caller is superadmin — which institute this subject belongs to. Ignored for institute-scoped admins (their own institute is stamped automatically).</summary>
    public int? InstituteId { get; set; }
}
