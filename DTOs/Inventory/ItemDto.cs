namespace SchoolManagement.API.DTOs.Inventory;

public class ItemDto
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int ItemCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Brand { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public string? Description { get; set; }

    public decimal MinimumStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal LastPurchasePrice { get; set; }
    public decimal AverageCost { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? MinimumSalePrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public bool IsActive { get; set; }

    public decimal QuantityOnHand { get; set; }
}

public class CreateItemDto
{
    public string? ItemCode { get; set; }               // null => auto-generated
    public string? Barcode { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int ItemCategoryId { get; set; }
    public string? Brand { get; set; }
    public int UnitId { get; set; }
    public string? Description { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? MinimumSalePrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal OpeningQuantity { get; set; }
    public decimal OpeningCost { get; set; }
}

public class UpdateItemDto
{
    public string ItemCode { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int ItemCategoryId { get; set; }
    public string? Brand { get; set; }
    public int UnitId { get; set; }
    public string? Description { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }
    public decimal? MinimumSalePrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public bool IsActive { get; set; }
}
