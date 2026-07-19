using System.Security.Claims;

namespace SchoolManagement.API.Infrastructure;

public interface ITenantContext
{
    int? InstituteId { get; }
    int? CampusId    { get; }
    bool IsSuperAdmin { get; }
}

public class TenantContext : ITenantContext
{
    public int? InstituteId { get; private set; }
    public int? CampusId    { get; private set; }
    public bool IsSuperAdmin { get; private set; }

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user == null) return;

        var role = user.FindFirstValue(ClaimTypes.Role) ?? "";
        IsSuperAdmin = role.Equals("superadmin", StringComparison.OrdinalIgnoreCase);

        if (int.TryParse(user.FindFirstValue("instituteId"), out var iid))
            InstituteId = iid;
        if (int.TryParse(user.FindFirstValue("campusId"), out var cid))
            CampusId = cid;
    }
}
