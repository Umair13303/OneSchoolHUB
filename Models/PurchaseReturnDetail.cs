namespace SchoolManagement.API.Models;

public class PurchaseReturnDetail : BaseEntity
{
    public int PurchaseReturnDetailId { get; set; }
    public int PurchaseReturnId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal NetAmount { get; set; }

    public PurchaseReturnMaster PurchaseReturn { get; set; } = null!;
    public ItemMaster Item { get; set; } = null!;
}
