namespace SchoolManagement.API.DTOs.User;

public class UserListDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Password { get; set; }

    // Teacher profile fields
    public string? Phone         { get; set; }
    public string? CNIC          { get; set; }
    public string? Gender        { get; set; }
    public string? Address       { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization{ get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? JoiningDate { get; set; }
    public string? SignatureUrl { get; set; }
}
