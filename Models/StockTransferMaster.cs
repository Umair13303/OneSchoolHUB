namespace SchoolManagement.API.Models;

/// <summary>
/// Stock transfer between locations/stores/campuses. FromLocation/ToLocation are
/// free-text labels (e.g. campus/branch name) — a dedicated Location master can
/// be introduced later without changing this shape (see "Multiple store locations"
/// future enhancement in the SRS).
/// </summary>
public class StockTransferMaster : BaseEntity
{
    public int StockTransferId { get; set; }
    public string TransferNo { get; set; } = string.Empty;
    public DateOnly TransferDate { get; set; }
    public string FromLocation { get; set; } = string.Empty;
    public string ToLocation { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public string Status { get; set; } = "Posted"; // Draft | Posted | Cancelled

    public ICollection<StockTransferDetail> Details { get; set; } = [];
}
