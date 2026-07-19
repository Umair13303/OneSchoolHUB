namespace SchoolManagement.API.DTOs.Staff;

public class StaffDto
{
    public int StaffId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Cnic { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateOnly? JoiningDate { get; set; }
    public string EmploymentType { get; set; } = "Permanent";
    public string Status { get; set; } = "Active";
    public int? PhotoFileId { get; set; }
    public int? UserId { get; set; }
    public string? UserEmail { get; set; }
}

public class CreateStaffDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Cnic { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string? Department { get; set; }
    public DateOnly? JoiningDate { get; set; }
    public string EmploymentType { get; set; } = "Permanent";
    public string Status { get; set; } = "Active";

    /// <summary>FileId of a photo already uploaded to the FileServer (entityType "staff-photo").</summary>
    public int? PhotoFileId { get; set; }
}

public class UpdateStaffDto : CreateStaffDto { }

public class CreateStaffLoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>One document attached to a staff member. Mirrors StudentDocumentDto, plus the admin-typed Label.</summary>
public class StaffDocumentDto
{
    public int      FileId       { get; set; }
    public string   Label        { get; set; } = string.Empty;
    public string   OriginalName { get; set; } = string.Empty;
    public string?  FileType     { get; set; }
    public long?    SizeBytes    { get; set; }
    public DateTime UploadedAt   { get; set; }
    public string   FileUrl      { get; set; } = string.Empty;
}

/// <summary>Body for tagging a just-uploaded FileServer file as belonging to this staff member's document list.</summary>
public class TagStaffDocumentDto
{
    public int    FileId { get; set; }
    public string Label  { get; set; } = string.Empty;
}
