using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolManagement.API.Infrastructure;

// Audits Created/Updated/Deleted changes on the main DbContext into the
// separate logging database. Changes must be captured in SavingChanges —
// by the time SavedChanges fires, EF has accepted all changes and every
// entry's state is back to Unchanged.
public class ActivityLogInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _http;
    private readonly LoggingDbContext _logDb;
    private readonly List<(EntityEntry Entry, string Action, string? OldJson, string? NewJson)> _pending = new();

    public ActivityLogInterceptor(IHttpContextAccessor http, LoggingDbContext logDb)
    {
        _http  = http;
        _logDb = logDb;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        Capture(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        Capture(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
    {
        FlushAsync().GetAwaiter().GetResult();
        return base.SavedChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        await FlushAsync(cancellationToken);
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override void SaveChangesFailed(DbContextErrorEventData eventData)
    {
        _pending.Clear();
        base.SaveChangesFailed(eventData);
    }

    public override Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        _pending.Clear();
        return base.SaveChangesFailedAsync(eventData, cancellationToken);
    }

    private void Capture(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            var action = entry.State switch
            {
                EntityState.Added    => "Created",
                EntityState.Modified => "Updated",
                EntityState.Deleted  => "Deleted",
                _                    => "Unknown"
            };

            string? oldJson = null;
            string? newJson = null;

            if (entry.State == EntityState.Modified)
            {
                var old = entry.Properties
                    .Where(p => p.IsModified && !IsSensitive(p.Metadata.Name))
                    .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                var @new = entry.Properties
                    .Where(p => p.IsModified && !IsSensitive(p.Metadata.Name))
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                oldJson = JsonSerializer.Serialize(old);
                newJson = JsonSerializer.Serialize(@new);
            }
            else if (entry.State == EntityState.Added)
            {
                var vals = entry.Properties
                    .Where(p => !IsSensitive(p.Metadata.Name))
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                newJson = JsonSerializer.Serialize(vals);
            }
            else if (entry.State == EntityState.Deleted)
            {
                var vals = entry.Properties
                    .Where(p => !IsSensitive(p.Metadata.Name))
                    .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                oldJson = JsonSerializer.Serialize(vals);
            }

            _pending.Add((entry, action, oldJson, newJson));
        }
    }

    private async Task FlushAsync(CancellationToken cancellationToken = default)
    {
        if (_pending.Count == 0) return;

        var user   = _http.HttpContext?.User;
        var userId = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? user?.FindFirstValue("sub");
        var userName  = user?.FindFirstValue(ClaimTypes.Name)    ?? "System";
        var userEmail = user?.FindFirstValue(ClaimTypes.Email)   ?? "";
        var userRole  = user?.FindFirstValue(ClaimTypes.Role)    ?? "";
        var ip        = _http.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        var ua        = _http.HttpContext?.Request?.Headers["User-Agent"].ToString();

        int.TryParse(user?.FindFirstValue("instituteId"), out var instId);

        foreach (var (entry, action, oldJson, newJson) in _pending)
        {
            // Read the key after the save so identity values generated by the
            // database are available for Created entries.
            var keyProp  = entry.Metadata.FindPrimaryKey()?.Properties.FirstOrDefault();
            var entityId = keyProp != null ? entry.Property(keyProp.Name).CurrentValue?.ToString() : null;

            _logDb.ActivityLogs.Add(new ActivityLog
            {
                UserId      = int.TryParse(userId, out var uid) ? uid : null,
                UserName    = userName,
                UserEmail   = userEmail,
                UserRole    = userRole,
                Action      = action,
                EntityName  = entry.Metadata.ClrType.Name,
                EntityId    = entityId,
                OldValues   = oldJson,
                NewValues   = newJson,
                IpAddress   = ip,
                UserAgent   = ua,
                Timestamp   = DateTime.UtcNow,
                InstituteId = instId == 0 ? null : instId
            });
        }

        _pending.Clear();
        await _logDb.SaveChangesAsync(cancellationToken);
    }

    private static bool IsSensitive(string propName) =>
        propName is "PasswordHash" or "Password" or "Token" or "RefreshToken";
}
