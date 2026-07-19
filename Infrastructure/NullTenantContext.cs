namespace SchoolManagement.API.Infrastructure;

/// <summary>
/// Fallback tenant context used during EF migrations and design-time tool
/// invocations when there is no HTTP request (and therefore no JWT).
/// Acts as superadmin so no tenant filter is applied.
/// </summary>
public sealed class NullTenantContext : ITenantContext
{
    public static readonly NullTenantContext Instance = new();
    public int?  InstituteId  => null;
    public int?  CampusId     => null;
    public bool  IsSuperAdmin => true;
}
