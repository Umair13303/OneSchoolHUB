using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Students;

/// <summary>
/// Body for <c>POST /api/students/{id}/transfer</c>.
/// Modes: Transfer (mid-year move), Promote (year-end), Readmit (after withdrawal).
/// </summary>
public class TransferStudentDto
{
    [Required] public int AcademicYearId { get; set; }
    [Required] public int ClassId { get; set; }

    /// <summary>Transfer | Promote | Readmit</summary>
    [Required, StringLength(20)]
    public string Mode { get; set; } = "Transfer";

    public DateOnly? EffectiveDate { get; set; }
}
