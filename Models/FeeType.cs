namespace SchoolManagement.API.Models;

/// <summary>
/// Recurring      – charged every cycle automatically (e.g. Tuition Fee)
/// OneTime        – charged once ever per student (e.g. Admission Fee)
/// OnDemand       – generated manually when needed (e.g. Exam Fee)
/// RefundableDeposit – collected on admission, refunded on leaving (e.g. Security Fee)
/// </summary>
public enum FeeCategory
{
    Recurring,
    OneTime,
    OnDemand,
    RefundableDeposit
}

public class FeeType : BaseEntity
{
    public int FeeTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FeeCategory FeeCategory { get; set; } = FeeCategory.Recurring;
    public bool IsActive { get; set; } = true;
}
