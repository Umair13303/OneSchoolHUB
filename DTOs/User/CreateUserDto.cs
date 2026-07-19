using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.API.DTOs.User;

public class CreateUserDto
{
    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    // Optional teacher profile fields
    public string? Phone         { get; set; }
    public string? CNIC          { get; set; }
    public string? Gender        { get; set; }
    public string? Address       { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization{ get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateOnly? JoiningDate { get; set; }
}
