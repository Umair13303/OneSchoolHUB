namespace SchoolManagement.API.DTOs.Fees;

public class StudentFeeDto
{
    public int StudentFeeId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public int FeeStructureId { get; set; }
    public string FeeTypeName { get; set; } = string.Empty;
    public int AcademicYearId { get; set; }
    public string YearLabel { get; set; } = string.Empty;
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal Discount { get; set; }
    public decimal Balance => AmountDue - Discount - AmountPaid;
    public DateOnly DueDate { get; set; }
    public int? FeeMonth { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public int? InstituteId { get; set; }
}

public class CreateStudentFeeDto
{
    public int StudentId { get; set; }
    public int FeeStructureId { get; set; }
    public int AcademicYearId { get; set; }
    public decimal AmountDue { get; set; }
    public decimal Discount { get; set; } = 0;
    public DateOnly DueDate { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateStudentFeeDto
{
    public decimal Discount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Remarks { get; set; }
}

public class BulkAssignFeeDto
{
    public int FeeStructureId { get; set; }
    public int ClassId { get; set; }
    public int AcademicYearId { get; set; }
    public DateOnly DueDate { get; set; }
    public int? Month { get; set; }   // billing month 1-12; enables per-month recurring challans
}

public class BulkAssignResultDto
{
    public int Created { get; set; }
    public int Skipped { get; set; }
    public List<StudentFeeDto> Fees { get; set; } = [];
}
