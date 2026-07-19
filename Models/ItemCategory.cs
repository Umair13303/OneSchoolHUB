namespace SchoolManagement.API.Models;

/// <summary>Inventory item category — Books, Copies, Stationery, Uniform, Bags, Shoes, Laboratory Items, Misc.</summary>
public class ItemCategory : BaseEntity
{
    public int ItemCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ItemMaster> Items { get; set; } = [];
}
