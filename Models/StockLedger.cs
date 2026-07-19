namespace SchoolManagement.API.Models;

/// <summary>
/// Full audit trail of every stock movement. One row per item per voucher line.
/// VoucherType: Purchase | Sale | PackageSale | Adjustment | Transfer | PurchaseReturn |
/// SalesReturn | Damage. Balance is the running quantity-on-hand for that item
/// immediately after this movement (per institute/campus tenant scope).
/// </summary>
public class StockLedger : BaseEntity
{
    public int StockLedgerId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string VoucherNo { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public decimal QtyIn { get; set; }
    public decimal QtyOut { get; set; }
    public decimal Balance { get; set; }
    public decimal Cost { get; set; }
    public int UserId { get; set; }

    public ItemMaster Item { get; set; } = null!;
}
