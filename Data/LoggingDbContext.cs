using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Data;

// Separate database for high-volume append-only log data, so the main
// business database stays small and can be backed up independently.
public class LoggingDbContext : DbContext
{
    private readonly ITenantContext _tenant;

    public LoggingDbContext(DbContextOptions<LoggingDbContext> options, ITenantContext? tenant = null) : base(options)
    {
        _tenant = tenant ?? NullTenantContext.Instance;
    }

    private bool IsSuperAdmin => _tenant.IsSuperAdmin;
    private int? TenantId     => _tenant.InstituteId;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ActivityLog>(e =>
        {
            e.HasQueryFilter(l => IsSuperAdmin || l.InstituteId == null || l.InstituteId == TenantId);
            e.HasIndex(l => l.Timestamp);
            e.HasIndex(l => l.InstituteId);
        });
    }
}
