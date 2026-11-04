using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

/// <summary>
/// Body for <c>PUT /api/students/{id}</c>. Updates editable profile fields
/// and optionally the primary guardian. Class changes use the transfer API.
/// AdmissionNo is immutable.
/// </summary>
public class UpdateStudentDto
{
    [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string LastName  { get; set; } = string.Empty;
    public DateOnly? DateOfBirth  { get; set; }
    [StringLength(20)]  public string? Gender      { get; set; }
    [StringLength(10)]  public string? BloodGroup  { get; set; }
    [StringLength(50)]  public string? Religion    { get; set; }
    [StringLength(50)]  public string? Nationality { get; set; }
    [StringLength(500)] public string? Address     { get; set; }
    [StringLength(30)]  public string? Phone       { get; set; }
    [EmailAddress, StringLength(150)] public string? Email { get; set; }

    public int? PhotoFileId { get; set; }
    public int? BirthCertificateFileId { get; set; }
    public int? PreviousSchoolCertFileId { get; set; }
    public bool IsActive    { get; set; } = true;

    /// <summary>When set, upserts the primary (first) guardian row. Prefer <see cref="Guardians"/>.</summary>
    public UpdateGuardianDto? Guardian { get; set; }

    /// <summary>Full guardian list for multi-guardian edit (replaces non-deleted rows by id / adds new).</summary>
    public List<UpdateGuardianDto>? Guardians { get; set; }
}

public class UpdateGuardianDto
{
    public int? GuardianId { get; set; }
    [Required, StringLength(150)] public string FullName { get; set; } = string.Empty;
    [StringLength(30)] public string? Relation { get; set; }
    [Required, StringLength(30)] public string Phone { get; set; } = string.Empty;
    [StringLength(30)] public string? CNIC { get; set; }
    [StringLength(100)] public string? Occupation { get; set; }
    /// <summary>When true on update, soft-deletes this guardian row.</summary>
    public bool Remove { get; set; }
}
