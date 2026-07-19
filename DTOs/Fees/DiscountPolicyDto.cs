namespace SchoolManagement.API.DTOs.Fees;

public class DiscountPolicyDto
{
    public int DiscountPolicyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ValueType { get; set; } = "Percentage";
    public decimal Value { get; set; }
    public int? MaxSiblingOrder { get; set; }
    public bool IsActive { get; set; }
}

public class CreateDiscountPolicyDto
{
    public string Name { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;  // Sibling | TeacherChild | Merit | NeedBased | FullWaiver | EarlyPayment | Custom
    public string Description { get; set; } = string.Empty;
    public string ValueType { get; set; } = "Percentage";     // Percentage | FixedAmount
    public decimal Value { get; set; }
    public int? MaxSiblingOrder { get; set; }
}

public class UpdateDiscountPolicyDto
{
    public string Name { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ValueType { get; set; } = "Percentage";
    public decimal Value { get; set; }
    public int? MaxSiblingOrder { get; set; }
    public bool IsActive { get; set; }
}

public class StudentDiscountDto
{
    public int StudentDiscountId { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public int DiscountPolicyId { get; set; }
    public string PolicyName { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public decimal PolicyValue { get; set; }
    public decimal? OverrideValue { get; set; }
    public decimal EffectiveValue { get; set; }   // OverrideValue ?? PolicyValue
    public int AcademicYearId { get; set; }
    public string YearLabel { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
}

public class AssignStudentDiscountDto
{
    public int StudentId { get; set; }
    public int DiscountPolicyId { get; set; }
    public int AcademicYearId { get; set; }
    public decimal? OverrideValue { get; set; }
    public string? Remarks { get; set; }
}

public class UpdateStudentDiscountDto
{
    public decimal? OverrideValue { get; set; }
    public string? Remarks { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>Returned by the fee generation preview to show discount breakdown per student.</summary>
public class FeeGenerationPreviewDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public decimal BaseFee { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal NetPayable { get; set; }
    public List<DiscountLineDto> DiscountLines { get; set; } = [];
    public bool AlreadyAssigned { get; set; }
}

public class DiscountLineDto
{
    public string PolicyName { get; set; } = string.Empty;
    public string DiscountType { get; set; } = string.Empty;
    public string ValueType { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal DiscountAmount { get; set; }
}
