namespace SchoolManagement.API.Models;

public class SalesReturnDetail : BaseEntity
{
    public int SalesReturnDetailId { get; set; }
    public int SalesReturnId { get; set; }
    /// <summary>The original SalesDetail line this return applies against — used to cap
    /// how much of that specific line can be returned (a plain reference, not an EF FK,
    /// since SalesDetail rows are not otherwise linked for cascade purposes).</summary>
    public int SalesDetailId { get; set; }
    public int? ItemId { get; set; }
    public int? PackageId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }

    public SalesReturnMaster SalesReturn { get; set; } = null!;
    public ItemMaster? Item { get; set; }
    public PackageMaster? Package { get; set; }
}
