namespace SchoolManagement.API.DTOs.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoDto User { get; set; } = null!;
}

public class UserInfoDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int? InstituteId { get; set; }
    public int? CampusId    { get; set; }
    public string InstituteName { get; set; } = "Dev_Solutions";
    public string? Tagline { get; set; }
    public string? LogoUrl { get; set; }
    public string? CopyrightText { get; set; }
}
