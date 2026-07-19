namespace SchoolManagement.API.Models;

public class Institute : BaseEntity
{
    public new int InstituteId { get; set; }  // PK, overrides BaseEntity.InstituteId
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // License validity: null = unlimited; past date = login blocked for this institute's users
    public DateTime? LicenseValidUntil { get; set; }

    // Module permissions (which modules this institute has access to)
    public bool ModuleAttendance { get; set; } = true;
    public bool ModuleFees       { get; set; } = true;
    public bool ModuleHomework   { get; set; } = true;
    public bool ModuleExams      { get; set; } = true;
    public bool ModuleTimetable  { get; set; } = true;
    public bool ModuleHR         { get; set; } = true;
    public bool ModuleReports    { get; set; } = true;

    // Challan print template assigned by superadmin
    public string ChallanTemplate { get; set; } = "cash_memo";
    public string? SchoolStampUrl { get; set; }

    public ICollection<Campus> Campuses { get; set; } = new List<Campus>();
}
