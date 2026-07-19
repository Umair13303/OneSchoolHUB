namespace SchoolManagement.API.Models;

/// <summary>
/// QtyDiff is signed: positive increases stock (found during physical count),
/// negative decreases stock (damage/expired/shrinkage). QtyBefore/QtyAfter are
/// captured for audit purposes.
/// </summary>
public class StockAdjustmentDetail : BaseEntity
{
    public int StockAdjustmentDetailId { get; set; }
    public int StockAdjustmentId { get; set; }
    public int ItemId { get; set; }
    public decimal QtyBefore { get; set; }
    public decimal QtyAfter { get; set; }
    public decimal QtyDiff { get; set; }
    public decimal Cost { get; set; }

    public StockAdjustmentMaster StockAdjustment { get; set; } = null!;
    public ItemMaster Item { get; set; } = null!;
}
