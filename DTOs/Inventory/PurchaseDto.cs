namespace SchoolManagement.API.DTOs.Inventory;

public class PurchaseDetailDto
{
    public int PurchaseDetailId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal NetAmount { get; set; }
}

public class PurchaseDto
{
    public int PurchaseId { get; set; }
    public string PurchaseNo { get; set; } = string.Empty;
    public DateOnly PurchaseDate { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? InvoiceNumber { get; set; }
    public string? Remarks { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<PurchaseDetailDto> Details { get; set; } = [];
}

public class CreatePurchaseDetailDto
{
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
}

public class CreatePurchaseDto
{
    public DateOnly PurchaseDate { get; set; }
    public int SupplierId { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Remarks { get; set; }
    public List<CreatePurchaseDetailDto> Details { get; set; } = [];
}
