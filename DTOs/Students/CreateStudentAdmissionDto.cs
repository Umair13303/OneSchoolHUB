using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

/// <summary>
/// Body for <c>POST /api/students</c> — the New Admission Form.
/// Creates the Student, its Guardians, and the initial StudentClassEnrollment
/// in a single transaction. AdmissionNo is generated server-side.
/// </summary>
public class CreateStudentAdmissionDto
{
    // ── Personal ──────────────────────────────────────────────────────────
    [Required, StringLength(100)] public string  FirstName   { get; set; } = string.Empty;
    [Required, StringLength(100)] public string  LastName    { get; set; } = string.Empty;
    public DateOnly? DateOfBirth  { get; set; }
    [StringLength(20)]  public string? Gender      { get; set; }
    [StringLength(10)]  public string? BloodGroup  { get; set; }
    [StringLength(50)]  public string? Religion    { get; set; }
    [StringLength(50)]  public string? Nationality { get; set; }

    // ── Contact ───────────────────────────────────────────────────────────
    [StringLength(500)] public string? Address { get; set; }
    [StringLength(30)]  public string? Phone   { get; set; }
    [EmailAddress, StringLength(150)] public string? Email { get; set; }

    // ── Admission ─────────────────────────────────────────────────────────
    /// <summary>Defaults to today (UTC) if not supplied.</summary>
    public DateOnly? AdmissionDate { get; set; }

    /// <summary>If provided, use this number instead of auto-generating one (for adding existing/old students).</summary>
    [StringLength(50)] public string? CustomAdmissionNo { get; set; }

    [Required] public int AcademicYearId { get; set; }

    // ── Class assignment (mandatory per task doc) ─────────────────────────
    [Required] public int ClassId { get; set; }

    // ── Documents (optional — uploaded separately, FileIds passed in) ─────
    public int? PhotoFileId               { get; set; }
    public int? BirthCertificateFileId    { get; set; }
    public int? PreviousSchoolCertFileId  { get; set; }

    // ── Guardians ─────────────────────────────────────────────────────────
    [Required, MinLength(1, ErrorMessage = "At least one guardian is required.")]
    public List<CreateGuardianDto> Guardians { get; set; } = new();

    // ── Fees at admission (optional) ───────────────────────────────────────
    public List<AdmissionFeeAssignmentDto> Fees { get; set; } = new();
}

/// <summary>
/// A fee structure to assign at admission time, with optional discount and payment status.
/// </summary>
public class AdmissionFeeAssignmentDto
{
    public int FeeStructureId { get; set; }
    public decimal AmountDue { get; set; }
    public decimal Discount { get; set; } = 0;
    public DateOnly DueDate { get; set; }
    public bool IsPaid { get; set; } = false;
    public decimal AmountPaid { get; set; } = 0;
    public string? Remarks { get; set; }
}
