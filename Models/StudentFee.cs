namespace SchoolManagement.API.Models;

/// <summary>
/// A fee invoice generated for one student for one fee structure period.
/// </summary>
public class StudentFee : BaseEntity
{
    public int StudentFeeId { get; set; }
    public int StudentId { get; set; }
    public int FeeStructureId { get; set; }
    public int AcademicYearId { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public DateOnly DueDate { get; set; }

    // Billing month (1-12) within the academic year. Null on legacy rows and
    // one-off fees created without a month. Recurring fees dedupe per month.
    public int? FeeMonth { get; set; }
    public string Status { get; set; } = "Unpaid";   // Unpaid | Partial | Paid | Waived
    public string? Remarks { get; set; }

    public Student Student { get; set; } = null!;
    public FeeStructure FeeStructure { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
    public ICollection<FeePayment> Payments { get; set; } = [];
}
