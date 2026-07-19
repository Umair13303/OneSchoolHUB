using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.ActivityLog;

namespace SchoolManagement.API.Services;

public interface IActivityLogService
{
    Task<ActivityLogPagedResult> GetLogsAsync(ActivityLogQuery query);
}

public class ActivityLogService : IActivityLogService
{
    private readonly LoggingDbContext _db;
    public ActivityLogService(LoggingDbContext db) => _db = db;

    public async Task<ActivityLogPagedResult> GetLogsAsync(ActivityLogQuery q)
    {
        var query = _db.ActivityLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim().ToLower();
            query = query.Where(l =>
                l.UserName.ToLower().Contains(s) ||
                l.UserEmail.ToLower().Contains(s) ||
                l.EntityName.ToLower().Contains(s) ||
                (l.EntityId != null && l.EntityId.Contains(s)));
        }

        if (!string.IsNullOrWhiteSpace(q.Action))
            query = query.Where(l => l.Action == q.Action);

        if (!string.IsNullOrWhiteSpace(q.UserRole))
            query = query.Where(l => l.UserRole == q.UserRole);

        if (!string.IsNullOrWhiteSpace(q.EntityName))
            query = query.Where(l => l.EntityName == q.EntityName);

        if (q.From.HasValue)
            query = query.Where(l => l.Timestamp >= q.From.Value.ToDateTime(TimeOnly.MinValue));

        if (q.To.HasValue)
            query = query.Where(l => l.Timestamp < q.To.Value.AddDays(1).ToDateTime(TimeOnly.MinValue));

        var total = await query.CountAsync();

        var page  = Math.Max(1, q.Page);
        var size  = Math.Clamp(q.PageSize, 10, 200);

        var items = await query
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * size)
            .Take(size)
            .Select(l => new ActivityLogDto
            {
                Id         = l.Id,
                UserId     = l.UserId,
                UserName   = l.UserName,
                UserEmail  = l.UserEmail,
                UserRole   = l.UserRole,
                Action     = l.Action,
                EntityName = l.EntityName,
                EntityId   = l.EntityId,
                OldValues  = l.OldValues,
                NewValues  = l.NewValues,
                IpAddress  = l.IpAddress,
                Timestamp  = l.Timestamp,
                InstituteId= l.InstituteId
            })
            .ToListAsync();

        return new ActivityLogPagedResult
        {
            Total    = total,
            Page     = page,
            PageSize = size,
            Items    = items
        };
    }
}
