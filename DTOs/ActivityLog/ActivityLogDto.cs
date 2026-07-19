namespace SchoolManagement.API.DTOs.ActivityLog;

public class ActivityLogDto
{
    public long    Id          { get; set; }
    public int?    UserId      { get; set; }
    public string  UserName    { get; set; } = string.Empty;
    public string  UserEmail   { get; set; } = string.Empty;
    public string  UserRole    { get; set; } = string.Empty;
    public string  Action      { get; set; } = string.Empty;
    public string  EntityName  { get; set; } = string.Empty;
    public string? EntityId    { get; set; }
    public string? OldValues   { get; set; }
    public string? NewValues   { get; set; }
    public string? IpAddress   { get; set; }
    public DateTime Timestamp  { get; set; }
    public int?    InstituteId { get; set; }
}

public class ActivityLogQuery
{
    public string? Search     { get; set; }
    public string? Action     { get; set; }
    public string? UserRole   { get; set; }
    public string? EntityName { get; set; }
    public DateOnly? From     { get; set; }
    public DateOnly? To       { get; set; }
    public int Page           { get; set; } = 1;
    public int PageSize       { get; set; } = 50;
}

public class ActivityLogPagedResult
{
    public int               Total    { get; set; }
    public int               Page     { get; set; }
    public int               PageSize { get; set; }
    public List<ActivityLogDto> Items { get; set; } = [];
}
