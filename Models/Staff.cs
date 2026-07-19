namespace SchoolManagement.API.Models;

public class Staff : BaseEntity
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
    public string EmploymentType { get; set; } = "Permanent"; // Permanent, Contract, DailyWage
    public string Status { get; set; } = "Active"; // Active, OnLeave, Terminated
    public int? PhotoFileId { get; set; }
    public int? UserId { get; set; } // optional system login

    public FileStore? Photo { get; set; }
    public User? User { get; set; }
}
