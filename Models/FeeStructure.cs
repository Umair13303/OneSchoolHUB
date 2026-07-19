namespace SchoolManagement.API.Models;

public class FeeStructure : BaseEntity
{
    public int FeeStructureId { get; set; }
    public int FeeTypeId { get; set; }
    public int ClassId { get; set; }
    public int AcademicYearId { get; set; }
    public decimal Amount { get; set; }
    public string DueDay { get; set; } = string.Empty;   // e.g. "Monthly", "Once", "Quarterly"
    public bool IsActive { get; set; } = true;

    public FeeType FeeType { get; set; } = null!;
    public Class Class { get; set; } = null!;
    public AcademicYear AcademicYear { get; set; } = null!;
}
