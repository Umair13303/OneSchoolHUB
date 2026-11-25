namespace SchoolManagement.API.Models;

/// <summary>
/// Manual student opening / outstanding balance (OB) for an academic year.
/// Entered on Fees → Student OB; counted in dues and fee reports.
/// </summary>
public class StudentOpeningBalance : BaseEntity
{
    public int StudentOpeningBalanceId { get; set; }
    public int StudentId { get; set; }
    public int AcademicYearId { get; set; }

    /// <summary>Original OB amount entered by staff.</summary>
    public decimal Amount { get; set; }

    /// <summary>Payments / adjustments applied against this OB.</summary>
    public decimal AmountPaid { get; set; } = 0;

    public DateOnly AsOfDate { get; set; }
    public string? Remarks { get; set; }

    /// <summary>Unpaid | Partial | Paid</summary>
    public string Status { get; set; } = "Unpaid";

    public Student Student { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
}
