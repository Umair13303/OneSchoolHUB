namespace SchoolManagement.API.Models;

/// <summary>
/// Item Master — master record for every sellable/purchasable inventory item
/// (books, stationery, uniforms, etc). Prices and costs are kept in sync by
/// PurchaseService (LastPurchasePrice / AverageCost) and are editable for
/// sale price only by authorized users at the controller/authorization level.
/// </summary>
public class ItemMaster : BaseEntity
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;      // auto or manual, unique
    public string? Barcode { get; set; }                      // unique when present
    public string ItemName { get; set; } = string.Empty;
    public int ItemCategoryId { get; set; }
    public string? Brand { get; set; }
    public int UnitId { get; set; }
    public string? Description { get; set; }

    public decimal MinimumStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }

    public decimal LastPurchasePrice { get; set; }
    public decimal AverageCost { get; set; }

    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? MinimumSalePrice { get; set; }

    public decimal TaxPercentage { get; set; }
    public bool IsActive { get; set; } = true;

    public ItemCategory Category { get; set; } = null!;
    public UnitMaster Unit { get; set; } = null!;
}
