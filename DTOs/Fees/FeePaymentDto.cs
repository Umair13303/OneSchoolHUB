namespace SchoolManagement.API.DTOs.Fees;

public class FeePaymentDto
{
    public int FeePaymentId { get; set; }
    public int StudentFeeId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly PaymentDate { get; set; }
    public string Method { get; set; } = string.Empty;
    public string? ReceiptNo { get; set; }
    public string? Remarks { get; set; }
    public string CollectedByName { get; set; } = string.Empty;
}

public class RecordPaymentDto
{
    public int StudentFeeId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateOnly PaymentDate { get; set; }
    public string Method { get; set; } = "Cash";
    public string? ReceiptNo { get; set; }
    public string? Remarks { get; set; }
}

public class FeeReportRowDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public decimal TotalDue { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal Balance => TotalDue - TotalDiscount - TotalPaid;
    public int UnpaidCount { get; set; }
    public int PaidCount { get; set; }
    public int PartialCount { get; set; }
}
