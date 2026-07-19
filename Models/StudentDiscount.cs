namespace SchoolManagement.API.Models;

/// <summary>
/// Assigns a discount policy to a specific student for a specific academic year.
/// Multiple discounts can be stacked; the fee generation sums them (capped at AmountDue).
/// </summary>
public class StudentDiscount : BaseEntity
{
    public int StudentDiscountId { get; set; }
    public int StudentId { get; set; }
    public int DiscountPolicyId { get; set; }
    public int AcademicYearId { get; set; }
    public decimal? OverrideValue { get; set; }   // Override policy value for this student (null = use policy default)
    public string? Remarks { get; set; }
    public bool IsActive { get; set; } = true;

    public Student Student { get; set; } = null!;
    public DiscountPolicy DiscountPolicy { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
}
