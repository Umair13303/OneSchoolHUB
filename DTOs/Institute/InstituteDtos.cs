namespace SchoolManagement.API.DTOs.Institute;

public class InstituteDto
{
    public int     InstituteId       { get; set; }
    public string  Name              { get; set; } = string.Empty;
    public string? Address           { get; set; }
    public string? Phone             { get; set; }
    public string? Email             { get; set; }
    public string? Website           { get; set; }
    public string? LogoUrl           { get; set; }
    public bool    IsActive          { get; set; }
    public DateTime? LicenseValidUntil { get; set; }
    public bool    ModuleAttendance  { get; set; }
    public bool    ModuleFees        { get; set; }
    public bool    ModuleHomework    { get; set; }
    public bool    ModuleExams       { get; set; }
    public bool    ModuleTimetable   { get; set; }
    public bool    ModuleHR          { get; set; }
    public bool    ModuleReports     { get; set; }
    public int     CampusCount       { get; set; }
    public string  ChallanTemplate   { get; set; } = "cash_memo";
    public string? SchoolStampUrl    { get; set; }
    public DateTime CreatedAt        { get; set; }
}

public class CreateInstituteDto
{
    public string  Name             { get; set; } = string.Empty;
    public string? Address          { get; set; }
    public string? Phone            { get; set; }
    public string? Email            { get; set; }
    public string? Website          { get; set; }
    public DateTime? LicenseValidUntil { get; set; }
    public bool    ModuleAttendance { get; set; } = true;
    public bool    ModuleFees       { get; set; } = true;
    public bool    ModuleHomework   { get; set; } = true;
    public bool    ModuleExams      { get; set; } = true;
    public bool    ModuleTimetable  { get; set; } = true;
    public bool    ModuleHR         { get; set; } = true;
    public bool    ModuleReports    { get; set; } = true;
}

public class UpdateInstituteDto : CreateInstituteDto
{
    public bool IsActive { get; set; } = true;
}

public class InstituteModulesDto
{
    public bool ModuleAttendance { get; set; }
    public bool ModuleFees       { get; set; }
    public bool ModuleHomework   { get; set; }
    public bool ModuleExams      { get; set; }
    public bool ModuleTimetable  { get; set; }
    public bool ModuleHR         { get; set; }
    public bool ModuleReports    { get; set; }
}

public class InstituteChallanTemplateDto
{
    public string ChallanTemplate { get; set; } = "cash_memo";
}
