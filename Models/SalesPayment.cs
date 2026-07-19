namespace SchoolManagement.API.Models;

/// <summary>Supports splitting one invoice across multiple payment methods.</summary>
public class SalesPayment : BaseEntity
{
    public int SalesPaymentId { get; set; }
    public int SalesId { get; set; }
    public string PaymentMethod { get; set; } = "Cash"; // Cash | Card | BankTransfer | DigitalWallet
    public decimal Amount { get; set; }
    public string? ReferenceNo { get; set; }

    public SalesMaster Sales { get; set; } = null!;
}
