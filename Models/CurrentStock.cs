namespace SchoolManagement.API.Models;

/// <summary>
/// Denormalized running balance per item, maintained transactionally alongside
/// StockLedger so stock lookups (POS search, low-stock, valuation) stay fast
/// (&lt;200ms per the NFRs) without summing the ledger on every read.
/// </summary>
public class CurrentStock : BaseEntity
{
    public int CurrentStockId { get; set; }
    public int ItemId { get; set; }
    public decimal QuantityOnHand { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    public ItemMaster Item { get; set; } = null!;
}
