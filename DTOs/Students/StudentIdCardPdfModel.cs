namespace SchoolManagement.API.DTOs.Students;

public class StudentIdCardPdfModel
{
    public string SchoolName { get; set; } = "School";
    public string? SchoolPhone { get; set; }
    public string? SchoolAddress { get; set; }
    public string? SchoolEmail { get; set; }
    public string? LogoUrl { get; set; }
    public byte[]? LogoBytes { get; set; }
    public byte[]? PhotoBytes { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string AdmissionNo { get; set; } = string.Empty;
    public string ClassLabel { get; set; } = "—";
    public string Gender { get; set; } = "—";
    public string BloodGroup { get; set; } = "—";
    public string Phone { get; set; } = "—";
    public string Address { get; set; } = "—";
    public string DateOfBirth { get; set; } = "—";
    public string GuardianName { get; set; } = "—";
    public string GuardianPhone { get; set; } = "—";
}
