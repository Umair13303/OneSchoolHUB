using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.Fees;

public class StudentOpeningBalanceDto
{
    public int StudentOpeningBalanceId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public string YearLabel { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Balance => Math.Max(0, Amount - AmountPaid);
    public DateOnly AsOfDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}

public class CreateStudentOpeningBalanceDto
{
    [Required] public int StudentId { get; set; }
    [Required] public int AcademicYearId { get; set; }
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    public decimal AmountPaid { get; set; } = 0;
    public DateOnly? AsOfDate { get; set; }
    [StringLength(500)] public string? Remarks { get; set; }
}

public class UpdateStudentOpeningBalanceDto
{
    [Range(0.01, double.MaxValue)] public decimal Amount { get; set; }
    [Range(0, double.MaxValue)] public decimal AmountPaid { get; set; }
    public DateOnly AsOfDate { get; set; }
    [StringLength(500)] public string? Remarks { get; set; }
}
