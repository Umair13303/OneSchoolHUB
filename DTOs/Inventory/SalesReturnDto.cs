namespace SchoolManagement.API.DTOs.Inventory;

public class SalesReturnDetailDto
{
    public int SalesDetailId { get; set; }
    public int? ItemId { get; set; }
    public int? PackageId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class SalesReturnDto
{
    public int SalesReturnId { get; set; }
    public string ReturnNo { get; set; } = string.Empty;
    public DateTime ReturnDate { get; set; }
    public int SalesId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public decimal NetAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<SalesReturnDetailDto> Details { get; set; } = [];
}

public class CreateSalesReturnLineDto
{
    public int SalesDetailId { get; set; }   // which original line is being returned
    public decimal Quantity { get; set; }
}

public class CreateSalesReturnDto
{
    public int SalesId { get; set; }
    public string? Remarks { get; set; }
    public List<CreateSalesReturnLineDto> Lines { get; set; } = [];
}
