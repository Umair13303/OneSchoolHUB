namespace SchoolManagement.API.Models;

/// <summary>Named tax rates (e.g. GST 17%, Sales Tax 5%) selectable on items/invoices.</summary>
public class TaxMaster : BaseEntity
{
    public int TaxId { get; set; }
    public string TaxName { get; set; } = string.Empty;   // e.g. "GST"
    public decimal TaxPercentage { get; set; }
    public bool IsActive { get; set; } = true;
}
