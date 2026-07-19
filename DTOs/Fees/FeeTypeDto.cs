namespace SchoolManagement.API.DTOs.Fees;

public class FeeTypeDto
{
    public int FeeTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FeeCategory { get; set; } = "Recurring";
    public bool IsActive { get; set; }
}

public class CreateFeeTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FeeCategory { get; set; } = "Recurring";
}

public class UpdateFeeTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FeeCategory { get; set; } = "Recurring";
    public bool IsActive { get; set; }
}
