namespace SchoolManagement.API.DTOs.Fees;

public class FeeStructureDto
{
    public int FeeStructureId { get; set; }
    public int FeeTypeId { get; set; }
    public string FeeTypeName { get; set; } = string.Empty;
    public string FeeCategory { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string? CampusName { get; set; }
    public int AcademicYearId { get; set; }
    public string YearLabel { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string DueDay { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

public class CreateFeeStructureDto
{
    public int FeeTypeId { get; set; }
    public int ClassId { get; set; }
    public int AcademicYearId { get; set; }
    public decimal Amount { get; set; }
    public string DueDay { get; set; } = string.Empty;
}

public class UpdateFeeStructureDto
{
    public decimal Amount { get; set; }
    public string DueDay { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
