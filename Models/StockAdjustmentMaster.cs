namespace SchoolManagement.API.Models;

/// <summary>
/// Covers manual Stock Adjustment, Damage Entry, Expired Stock and Physical
/// Stock Verification — all represented as one header + item lines that record
/// the qty change to apply against CurrentStock/StockLedger.
/// AdjustmentType: Adjustment | Damage | Expired | PhysicalVerification
/// </summary>
public class StockAdjustmentMaster : BaseEntity
{
    public int StockAdjustmentId { get; set; }
    public string AdjustmentNo { get; set; } = string.Empty;
    public DateOnly AdjustmentDate { get; set; }
    public string AdjustmentType { get; set; } = "Adjustment";
    public string? Remarks { get; set; }
    public string Status { get; set; } = "Posted"; // Draft | Posted | Cancelled

    public ICollection<StockAdjustmentDetail> Details { get; set; } = [];
}
