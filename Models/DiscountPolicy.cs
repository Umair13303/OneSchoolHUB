namespace SchoolManagement.API.Models;

/// <summary>
/// A named discount rule that can be assigned to students.
/// DiscountType values: Sibling, TeacherChild, Merit, NeedBased, FullWaiver, EarlyPayment, Custom
/// ValueType: Percentage | FixedAmount
/// </summary>
public class DiscountPolicy : BaseEntity
{
    public int DiscountPolicyId { get; set; }
    public string Name { get; set; } = string.Empty;          // e.g. "Sibling Discount – 2nd Child"
    public string DiscountType { get; set; } = string.Empty;  // Sibling | TeacherChild | Merit | NeedBased | FullWaiver | EarlyPayment | Custom
    public string Description { get; set; } = string.Empty;
    public string ValueType { get; set; } = "Percentage";     // Percentage | FixedAmount
    public decimal Value { get; set; }                        // e.g. 25 (= 25%) or 500 (= PKR 500)
    public int? MaxSiblingOrder { get; set; }                 // For Sibling type: applies to 2nd, 3rd... child (null = any)
    public bool IsActive { get; set; } = true;

    public ICollection<StudentDiscount> StudentDiscounts { get; set; } = [];
}
