namespace SchoolManagement.API.DTOs.Inventory;

// ── Item Category ────────────────────────────────────────────────────────────
public class ItemCategoryDto
{
    public int ItemCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int ItemCount { get; set; }
}
public class SaveItemCategoryDto
{
    public string CategoryName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

// ── Unit ──────────────────────────────────────────────────────────────────────
public class UnitDto
{
    public int UnitId { get; set; }
    public string UnitName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
public class SaveUnitDto
{
    public string UnitName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

// ── Tax ───────────────────────────────────────────────────────────────────────
public class TaxDto
{
    public int TaxId { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public bool IsActive { get; set; }
}
public class SaveTaxDto
{
    public string TaxName { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public bool IsActive { get; set; } = true;
}
