using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

/// <summary>
/// Body for <c>PUT /api/students/{id}</c>. Updates editable profile fields
/// only — class transfer is a separate (future) flow because it has
/// promotion/withdrawal semantics. AdmissionNo is immutable.
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
    public bool IsActive    { get; set; } = true;
}
