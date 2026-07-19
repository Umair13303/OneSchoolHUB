namespace SchoolManagement.API.Models;

/// <summary>A bundle of items sold as one product (e.g. "Class 1 Complete Book Set").</summary>
public class PackageMaster : BaseEntity
{
    public int PackageId { get; set; }
    public string PackageCode { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public decimal PackagePrice { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<PackageDetail> Details { get; set; } = [];
}
