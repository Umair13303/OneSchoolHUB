using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Infrastructure;

/// <summary>
/// Automatically stamps InstituteId and CampusId on every new entity that
/// inherits BaseEntity (or has those properties) before it is inserted.
/// Superadmin creates are not stamped — they operate without a tenant context.
/// </summary>
public class TenantStampInterceptor : SaveChangesInterceptor
{
    private readonly ITenantContext _tenant;

    public TenantStampInterceptor(ITenantContext tenant) => _tenant = tenant;

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, InterceptionResult<int> result)
    {
        Stamp(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        Stamp(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void Stamp(DbContext? context)
    {
        if (context == null) return;
        if (_tenant.IsSuperAdmin) return;
        if (!_tenant.InstituteId.HasValue) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added))
        {
            // Only stamp if not already set (allow explicit overrides)
            if (entry.Entity.InstituteId == null)
                entry.Entity.InstituteId = _tenant.InstituteId;
            if (entry.Entity.CampusId == null && _tenant.CampusId.HasValue)
                entry.Entity.CampusId = _tenant.CampusId;
        }
    }
}
