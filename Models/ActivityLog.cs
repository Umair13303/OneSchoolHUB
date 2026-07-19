namespace SchoolManagement.API.Models;

public class ActivityLog
{
    public long   Id          { get; set; }
    public int?   UserId      { get; set; }
    public string UserName    { get; set; } = string.Empty;
    public string UserEmail   { get; set; } = string.Empty;
    public string UserRole    { get; set; } = string.Empty;
    public string Action      { get; set; } = string.Empty;   // Created | Updated | Deleted | Login | Logout
    public string EntityName  { get; set; } = string.Empty;   // e.g. Student, User, Attendance
    public string? EntityId   { get; set; }
    public string? OldValues  { get; set; }                   // JSON snapshot before change
    public string? NewValues  { get; set; }                   // JSON snapshot after change
    public string? IpAddress  { get; set; }
    public string? UserAgent  { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int?   InstituteId { get; set; }
}
