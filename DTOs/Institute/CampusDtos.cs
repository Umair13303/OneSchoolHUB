namespace SchoolManagement.API.DTOs.Institute;

public class CampusDto
{
    public int     CampusId    { get; set; }
    public int     InstituteId { get; set; }
    public string  Name        { get; set; } = string.Empty;
    public string? Address     { get; set; }
    public string? Phone       { get; set; }
    public bool    IsActive    { get; set; }
    public int     UserCount   { get; set; }
    public DateTime CreatedAt  { get; set; }
}

public class CreateCampusDto
{
    public string  Name    { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone   { get; set; }
}

public class UpdateCampusDto : CreateCampusDto
{
    public bool IsActive { get; set; } = true;
}

public class CreateCampusAdminDto
{
    public string  FullName { get; set; } = string.Empty;
    public string  Email    { get; set; } = string.Empty;
    public string  Password { get; set; } = string.Empty;
    public int?    CampusId { get; set; }
}
