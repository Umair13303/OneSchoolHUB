namespace SchoolManagement.API.Models;

public class FeePayment : BaseEntity
{
    public int FeePaymentId { get; set; }
    public int StudentFeeId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly PaymentDate { get; set; }
    public string Method { get; set; } = "Cash";   // Cash | Bank | Online
    public string? ReceiptNo { get; set; }
    public string? Remarks { get; set; }
    public int CollectedBy { get; set; }           // UserId

    public StudentFee StudentFee { get; set; } = null!;
    public User CollectedByUser { get; set; } = null!;
}
