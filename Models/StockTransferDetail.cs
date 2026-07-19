namespace SchoolManagement.API.Models;

public class StockTransferDetail : BaseEntity
{
    public int StockTransferDetailId { get; set; }
    public int StockTransferId { get; set; }
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }

    public StockTransferMaster StockTransfer { get; set; } = null!;
    public ItemMaster Item { get; set; } = null!;
}
