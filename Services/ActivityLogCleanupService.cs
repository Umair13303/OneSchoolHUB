using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;

namespace SchoolManagement.API.Services;

// Nightly retention job for the logging database. Deletes activity logs older
// than ActivityLogs:RetentionDays (default 365). Set to 0 to keep logs forever.
public class ActivityLogCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<ActivityLogCleanupService> _logger;

    public ActivityLogCleanupService(
        IServiceScopeFactory scopeFactory,
        IConfiguration config,
        ILogger<ActivityLogCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var retentionDays = _config.GetValue<int?>("ActivityLogs:RetentionDays") ?? 365;
                if (retentionDays > 0)
                {
                    var cutoff = DateTime.UtcNow.AddDays(-retentionDays);
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();
                    var deleted = await db.ActivityLogs
                        .IgnoreQueryFilters()
                        .Where(l => l.Timestamp < cutoff)
                        .ExecuteDeleteAsync(stoppingToken);
                    if (deleted > 0)
                        _logger.LogInformation("Activity log cleanup removed {Count} rows older than {Cutoff:d}.", deleted, cutoff);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Activity log cleanup failed; will retry on next run.");
            }

            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
