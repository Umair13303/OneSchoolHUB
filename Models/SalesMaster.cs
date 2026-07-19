namespace SchoolManagement.API.Models;

/// <summary>
/// POS sale invoice header. Status: Held | Completed | Returned | PartiallyReturned | Cancelled.
/// A "Held" invoice has no stock impact until resumed and completed.
/// </summary>
public class SalesMaster : BaseEntity
{
    public int SalesId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime SalesDate { get; set; }

    public int? StudentId { get; set; }
    public string? CustomerName { get; set; }

    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }

    public string Status { get; set; } = "Completed"; // Held | Completed | Returned | PartiallyReturned | Cancelled
    public int CashierId { get; set; }
    public string? Remarks { get; set; }
    public string? HoldReference { get; set; }         // label used to find held invoices ("Table 3", customer name, etc.)

    public ICollection<SalesDetail> Details { get; set; } = [];
    public ICollection<SalesPayment> Payments { get; set; } = [];
}
