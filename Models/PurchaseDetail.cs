namespace SchoolManagement.API.Models;

public class PurchaseDetail : BaseEntity
{
    public int PurchaseDetailId { get; set; }
    public int PurchaseId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal NetAmount { get; set; }

    public PurchaseMaster Purchase { get; set; } = null!;
    public ItemMaster Item { get; set; } = null!;
}
