namespace SchoolManagement.API.DTOs.Inventory;

/// <summary>Lightweight shape used by the POS item/barcode search box.</summary>
public class PosLookupDto
{
    public string Kind { get; set; } = "Item"; // Item | Package
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;   // ItemCode / PackageCode
    public string? Barcode { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal QuantityOnHand { get; set; }         // not meaningful for packages
}

public class SalesDetailDto
{
    public int SalesDetailId { get; set; }
    public int? ItemId { get; set; }
    public int? PackageId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Amount { get; set; }
}

public class SalesPaymentDto
{
    public int SalesPaymentId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
}

public class SalesDto
{
    public int SalesId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime SalesDate { get; set; }
    public int? StudentId { get; set; }
    public string? StudentName { get; set; }
    public string? CustomerName { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CashierId { get; set; }
    public string CashierName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string? HoldReference { get; set; }
    public List<SalesDetailDto> Details { get; set; } = [];
    public List<SalesPaymentDto> Payments { get; set; } = [];
}

public class CreateSaleLineDto
{
    public string Kind { get; set; } = "Item"; // Item | Package
    public int Id { get; set; }                // ItemId or PackageId
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }         // editable at POS (defaults to sale/package price client-side)
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
}

public class CreatePaymentLineDto
{
    public string PaymentMethod { get; set; } = "Cash";
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }
}

/// <summary>Body for POST /api/pos/sales — a completed (or held) checkout.</summary>
public class CreateSaleDto
{
    public int? StudentId { get; set; }
    public string? CustomerName { get; set; }
    public string? Remarks { get; set; }
    public string? HoldReference { get; set; }
    public bool Hold { get; set; } = false;      // true => save as "Held", no stock/payment posted yet
    public List<CreateSaleLineDto> Lines { get; set; } = [];
    public List<CreatePaymentLineDto> Payments { get; set; } = [];
}

/// <summary>Body for POST /api/pos/sales/{id}/resume — completes a previously held invoice.</summary>
public class CompleteSaleDto
{
    public List<CreateSaleLineDto>? Lines { get; set; }  // optional: allows edits before completing
    public List<CreatePaymentLineDto> Payments { get; set; } = [];
}
