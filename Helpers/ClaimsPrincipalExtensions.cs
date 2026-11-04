using System.Security.Claims;

namespace SchoolManagement.API.Helpers;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// True when the caller is a pure parent (not also teacher/admin/staff).
    /// Dual teacher+parent users must NOT be forced through child-only class scoping
    /// on attendance/homework/timetable endpoints they use for teaching.
    /// </summary>
    public static bool IsParentOnly(this ClaimsPrincipal user)
    {
        if (!user.IsInRole("parent")) return false;
        return !user.IsInRole("teacher")
            && !user.IsInRole("admin")
            && !user.IsInRole("principal")
            && !user.IsInRole("superadmin")
            && !user.IsInRole("staff");
    }

    /// <summary>
    /// Fee list/detail must be scoped to linked children for pure parents and
    /// teacher+parent. Admin/principal/superadmin keep school-wide fee access
    /// even when they are also linked as a parent.
    /// </summary>
    public static bool IsFeeScopedToOwnChildren(this ClaimsPrincipal user)
    {
        if (!user.IsInRole("parent")) return false;
        return !user.IsInRole("superadmin")
            && !user.IsInRole("admin")
            && !user.IsInRole("principal");
    }

    public static IReadOnlyList<int> GetRoleIds(this ClaimsPrincipal user)
    {
        var ids = user.FindAll("roleId")
            .Select(c => int.TryParse(c.Value, out var id) ? id : (int?)null)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Distinct()
            .ToList();
        return ids;
    }
}
