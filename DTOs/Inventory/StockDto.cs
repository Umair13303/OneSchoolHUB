namespace SchoolManagement.API.DTOs.Inventory;

public class CurrentStockDto
{
    public int ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal QuantityOnHand { get; set; }
    public decimal MinimumStockLevel { get; set; }
    public decimal ReorderLevel { get; set; }
    public decimal AverageCost { get; set; }
    public decimal StockValue { get; set; }
    public DateTime LastUpdated { get; set; }
}

public class StockLedgerDto
{
    public int StockLedgerId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string VoucherNo { get; set; } = string.Empty;
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal QtyIn { get; set; }
    public decimal QtyOut { get; set; }
    public decimal Balance { get; set; }
    public decimal Cost { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class StockAdjustmentDetailDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal QtyBefore { get; set; }
    public decimal QtyAfter { get; set; }
    public decimal QtyDiff { get; set; }
    public decimal Cost { get; set; }
}

public class StockAdjustmentDto
{
    public int StockAdjustmentId { get; set; }
    public string AdjustmentNo { get; set; } = string.Empty;
    public DateOnly AdjustmentDate { get; set; }
    public string AdjustmentType { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<StockAdjustmentDetailDto> Details { get; set; } = [];
}

public class CreateStockAdjustmentLineDto
{
    public int ItemId { get; set; }
    /// <summary>New physical/actual quantity. Diff against current on-hand is computed server-side.</summary>
    public decimal NewQuantity { get; set; }
}

public class CreateStockAdjustmentDto
{
    public DateOnly AdjustmentDate { get; set; }
    public string AdjustmentType { get; set; } = "Adjustment"; // Adjustment | Damage | Expired | PhysicalVerification
    public string? Remarks { get; set; }
    public List<CreateStockAdjustmentLineDto> Lines { get; set; } = [];
}

public class StockTransferDetailDto
{
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
}

public class StockTransferDto
{
    public int StockTransferId { get; set; }
    public string TransferNo { get; set; } = string.Empty;
    public DateOnly TransferDate { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<StockTransferDetailDto> Details { get; set; } = [];
}

public class CreateStockTransferLineDto
{
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
}

public class CreateStockTransferDto
{
    public DateOnly TransferDate { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public List<CreateStockTransferLineDto> Lines { get; set; } = [];
}
