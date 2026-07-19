namespace SchoolManagement.API.Models;

/// <summary>Unit of measure for inventory items (Piece, Dozen, Ream, Box, etc.).</summary>
public class UnitMaster : BaseEntity
{
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;   // e.g. "Piece"
    public string ShortName { get; set; } = string.Empty;  // e.g. "Pc"
    public bool IsActive { get; set; } = true;

    public ICollection<ItemMaster> Items { get; set; } = [];
}
