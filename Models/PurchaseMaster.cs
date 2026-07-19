namespace SchoolManagement.API.Models;

/// <summary>
/// Purchase invoice header. Status: Draft | Posted | Cancelled.
/// Posting a purchase increases stock, updates LastPurchasePrice/AverageCost on
/// each item, and writes StockLedger entries (handled by PurchaseService + StockService).
/// </summary>
public class PurchaseMaster : BaseEntity
{
    public int PurchaseId { get; set; }
    public string PurchaseNo { get; set; } = string.Empty;   // auto-generated, unique
    public DateOnly PurchaseDate { get; set; }
    public int SupplierId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Remarks { get; set; }

    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }

    public string Status { get; set; } = "Posted";  // Draft | Posted | Cancelled

    public Supplier Supplier { get; set; } = null!;
    public ICollection<PurchaseDetail> Details { get; set; } = [];
}
