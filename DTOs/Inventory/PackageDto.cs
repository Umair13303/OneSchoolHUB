namespace SchoolManagement.API.DTOs.Inventory;

public class PackageDetailDto
{
    public int PackageDetailId { get; set; }
    public int ItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Quantity { get; set; }
    public decimal ItemSalePrice { get; set; }
}

public class PackageDto
{
    public int PackageId { get; set; }
    public string PackageCode { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public decimal PackagePrice { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<PackageDetailDto> Details { get; set; } = [];
}

public class PackageLineDto
{
    public int ItemId { get; set; }
    public decimal Quantity { get; set; }
}

public class CreatePackageDto
{
    public string? PackageCode { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public decimal PackagePrice { get; set; }
    public string? Description { get; set; }
    public List<PackageLineDto> Details { get; set; } = [];
}

public class UpdatePackageDto
{
    public string PackageName { get; set; } = string.Empty;
    public decimal PackagePrice { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public List<PackageLineDto> Details { get; set; } = [];
}
