namespace SchoolManagement.API.DTOs.Inventory;

public class InventorySettingsDto
{
    public int InventorySettingsId { get; set; }
    public string CostingMethod { get; set; } = "WeightedAverage";
    public bool BarcodeEnabled { get; set; }
    public bool PackageEnabled { get; set; }
    public bool StudentSalesEnabled { get; set; }
    public bool NegativeStockAllowed { get; set; }
    public bool AutoInvoiceNumber { get; set; }
    public bool AutoBarcode { get; set; }
    public decimal DefaultTaxPercentage { get; set; }
    public string DefaultCurrency { get; set; } = "PKR";
}

public class UpdateInventorySettingsDto
{
    public string CostingMethod { get; set; } = "WeightedAverage"; // LastPurchaseCost | WeightedAverage
    public bool BarcodeEnabled { get; set; }
    public bool PackageEnabled { get; set; }
    public bool StudentSalesEnabled { get; set; }
    public bool NegativeStockAllowed { get; set; }
    public bool AutoInvoiceNumber { get; set; }
    public bool AutoBarcode { get; set; }
    public decimal DefaultTaxPercentage { get; set; }
    public string DefaultCurrency { get; set; } = "PKR";
}
