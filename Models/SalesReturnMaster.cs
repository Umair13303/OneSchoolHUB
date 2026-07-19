namespace SchoolManagement.API.Models;

/// <summary>Return of sold items. Increases stock and reverses stock movement on posting.</summary>
public class SalesReturnMaster : BaseEntity
{
    public int SalesReturnId { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public int SalesId { get; set; }
    public string? Remarks { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = "Posted"; // Posted | Cancelled

    public SalesMaster Sales { get; set; } = null!;
    public ICollection<SalesReturnDetail> Details { get; set; } = [];
}
