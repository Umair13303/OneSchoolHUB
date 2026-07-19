using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

public class CreateGuardianDto
{
    [Required, StringLength(150)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? Relation { get; set; }       // Father / Mother / Guardian

    [StringLength(30)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Occupation { get; set; }

    [StringLength(30)]
    public string? CNIC { get; set; }

    /// <summary>Optional — link to an existing Parent user account.</summary>
    public int? UserId { get; set; }
}
