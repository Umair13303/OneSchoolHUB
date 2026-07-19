namespace SchoolManagement.API.Models;

/// <summary>Return of purchased items to a supplier. Reduces stock on posting.</summary>
public class PurchaseReturnMaster : BaseEntity
{
    public int PurchaseReturnId { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public DateOnly ReturnDate { get; set; }
    public int? PurchaseId { get; set; }
    public int SupplierId { get; set; }
    public string? Remarks { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = "Posted"; // Draft | Posted | Cancelled

    public PurchaseMaster? Purchase { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public ICollection<PurchaseReturnDetail> Details { get; set; } = [];
}
