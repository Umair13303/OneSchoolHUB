namespace SchoolManagement.API.Models;

/// <summary>
/// One line of a POS sale. Either ItemId or PackageId is set (not both).
/// When PackageId is set, PosService also deducts stock for every item inside
/// the package (see PackageDetail) without creating separate visible lines.
/// </summary>
public class SalesDetail : BaseEntity
{
    public int SalesDetailId { get; set; }
    public int SalesId { get; set; }
    public int? ItemId { get; set; }
    public int? PackageId { get; set; }
    public string ItemName { get; set; } = string.Empty; // snapshot at time of sale
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Amount { get; set; }

    /// <summary>
    /// Cost basis snapshot for this line (Quantity × AverageCost at time of sale,
    /// summed across constituent items when PackageId is set). Captured at sale
    /// time so Gross Profit / Profit-by-Item reports stay accurate even after
    /// AverageCost later changes.
    /// </summary>
    public decimal CostAmount { get; set; }

    public SalesMaster Sales { get; set; } = null!;
    public ItemMaster? Item { get; set; }
    public PackageMaster? Package { get; set; }
}
