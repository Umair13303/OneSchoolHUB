namespace SchoolManagement.API.DTOs.User;

public class UpdateMyProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? CNIC { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
}

public class ChangeMyPasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class UpdateMyPhotoDto
{
    public int PhotoFileId { get; set; }
}
