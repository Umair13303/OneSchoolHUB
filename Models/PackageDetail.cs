namespace SchoolManagement.API.Models;

public class PackageDetail : BaseEntity
{
    public int PackageDetailId { get; set; }
    public int PackageId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }

    public PackageMaster Package { get; set; } = null!;
    public ItemMaster Item { get; set; } = null!;
}
