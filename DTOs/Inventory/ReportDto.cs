namespace SchoolManagement.API.DTOs.Inventory;

// ── Purchase Reports ──────────────────────────────────────────────────────────
/// <summary>One row per purchase invoice — used for Purchase Summary and (grouped) Supplier Purchase Report.</summary>
public class PurchaseSummaryRowDto
{
    public int PurchaseId { get; set; }
    public string PurchaseNo { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
}

/// <summary>Line-level detail — Purchase Detail report.</summary>
public class PurchaseDetailRowDto
{
    public DateOnly PurchaseDate { get; set; }
    public string PurchaseNo { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal NetAmount { get; set; }
}

// ── Sales Reports (Daily / Monthly / By Item / By Student / By User) ─────────
/// <summary>Generic aggregated sales row — the "Label" meaning depends on the grouping requested.</summary>
public class SalesSummaryRowDto
{
    public string Label { get; set; } = string.Empty; // date, item name, student name, or cashier name
    public decimal Quantity { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public int InvoiceCount { get; set; }
}

// ── Inventory Reports ─────────────────────────────────────────────────────────
public class StockValuationRowDto
{
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal QuantityOnHand { get; set; }
    public decimal AverageCost { get; set; }
    public decimal StockValue { get; set; }
}

// (Stock Balance / Low Stock / Out of Stock reuse CurrentStockDto from StockDto.cs)

// ── Financial Reports ─────────────────────────────────────────────────────────
/// <summary>Generic profit row — the "Label" meaning depends on grouping (overall / item / category).</summary>
public class ProfitRowDto
{
    public string Label { get; set; } = string.Empty;
    public decimal QuantitySold { get; set; }
    public decimal SaleAmount { get; set; }
    public decimal CostAmount { get; set; }
    public decimal Profit { get; set; }
    public decimal MarginPercent { get; set; }
}

// ── Package Reports ────────────────────────────────────────────────────────────
public class PackageSalesRowDto
{
    public int PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public decimal QuantitySold { get; set; }
    public decimal NetAmount { get; set; }
    public int InvoiceCount { get; set; }
}
