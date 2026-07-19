namespace SchoolManagement.API.DTOs.Inventory;

public class PurchaseReturnDetailDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal NetAmount { get; set; }
}

public class PurchaseReturnDto
{
    public int PurchaseReturnId { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public DateOnly ReturnDate { get; set; }
    public int? PurchaseId { get; set; }
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<PurchaseReturnDetailDto> Details { get; set; } = [];
}

public class CreatePurchaseReturnDetailDto
{
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}

public class CreatePurchaseReturnDto
{
    public DateOnly ReturnDate { get; set; }
    public int? PurchaseId { get; set; }
    public int SupplierId { get; set; }
    public string? Remarks { get; set; }
    public List<CreatePurchaseReturnDetailDto> Details { get; set; } = [];
}
