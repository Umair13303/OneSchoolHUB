namespace SchoolManagement.API.DTOs.Students;

public class GuardianDto
{
    public int    GuardianId  { get; set; }
    public string FullName    { get; set; } = string.Empty;
    public string? Relation   { get; set; }     // Father, Mother, Guardian
    public string? Phone      { get; set; }
    public string? Occupation { get; set; }
    public string? CNIC       { get; set; }
    public int?   UserId      { get; set; }     // Optional link to a Parent user account
}
