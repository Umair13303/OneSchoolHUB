namespace SchoolManagement.API.Models;

public class DevCompany
{
    public int Id { get; set; }
    public string Name { get; set; } = "Dev_Solutions";
    public string? Tagline { get; set; }
    public string? LogoUrl { get; set; }
    public string? CopyrightText { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
