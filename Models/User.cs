namespace SchoolManagement.API.Models;

public class User : BaseEntity
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public bool IsActive { get; set; } = true;

    // Teacher profile fields (nullable — only populated for teacher role)
    public string? Phone         { get; set; }
    public string? CNIC          { get; set; }
    public string? Gender        { get; set; }
    public string? Address       { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization{ get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? JoiningDate { get; set; }
    public string? SignatureUrl { get; set; }

    public Role Role { get; set; } = null!;
    public Institute? Institute { get; set; }
    public Campus?    Campus    { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
