namespace SchoolManagement.API.Models;

/// <summary>
/// One configuration row per institute (or one global row when InstituteId is null,
/// used for superadmin/system defaults). CostingMethod: LastPurchaseCost | WeightedAverage.
/// </summary>
public class InventorySettings : BaseEntity
{
    public int InventorySettingsId { get; set; }

    public string CostingMethod { get; set; } = "WeightedAverage"; // LastPurchaseCost | WeightedAverage
    public bool BarcodeEnabled { get; set; } = true;
    public bool PackageEnabled { get; set; } = true;
    public bool StudentSalesEnabled { get; set; } = true;
    public bool NegativeStockAllowed { get; set; } = false;
    public bool AutoInvoiceNumber { get; set; } = true;
    public bool AutoBarcode { get; set; } = false;
    public decimal DefaultTaxPercentage { get; set; } = 0;
    public string DefaultCurrency { get; set; } = "PKR";
}
