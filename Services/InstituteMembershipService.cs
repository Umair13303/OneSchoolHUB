using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IInstituteMembershipService
{
    /// <summary>
    /// Resolve the school tenant for a user. When <paramref name="heal"/> is true and a
    /// missing InstituteId can be inferred, persist it on the Users row.
    /// </summary>
    Task<InstituteMembership?> ResolveAsync(
        int userId,
        int? currentInstituteId,
        int? currentCampusId,
        string roleName,
        bool heal = true);
}

public sealed record InstituteMembership(int InstituteId, int? CampusId, Institute Institute);

public class InstituteMembershipService : IInstituteMembershipService
{
    private readonly AppDbContext _db;

    public InstituteMembershipService(AppDbContext db) => _db = db;

    public async Task<InstituteMembership?> ResolveAsync(
        int userId,
        int? currentInstituteId,
        int? currentCampusId,
        string roleName,
        bool heal = true)
    {
        if (roleName.Equals("superadmin", StringComparison.OrdinalIgnoreCase))
            return null;

        int? instituteId = currentInstituteId;
        int? campusId = currentCampusId;

        if (!instituteId.HasValue)
            (instituteId, campusId) = await InferAsync(userId, campusId);

        if (!instituteId.HasValue)
            return null;

        if (heal && currentInstituteId != instituteId)
            await HealUserAsync(userId, instituteId.Value, campusId);

        var inst = await _db.Institutes.AsNoTracking()
            .IgnoreQueryFilters()
            .Where(i => i.InstituteId == instituteId.Value)
            .Select(i => new Institute
            {
                InstituteId = i.InstituteId,
                Name = i.Name,
                LogoUrl = i.LogoUrl,
                LicenseValidUntil = i.LicenseValidUntil
            })
            .FirstOrDefaultAsync();

        return inst == null ? null : new InstituteMembership(instituteId.Value, campusId, inst);
    }

    private async Task<(int? InstituteId, int? CampusId)> InferAsync(int userId, int? campusId)
    {
        // 1) Staff login link
        var staff = await _db.Staff.AsNoTracking().IgnoreQueryFilters()
            .Where(s => s.UserId == userId && !s.IsDeleted && s.InstituteId != null)
            .Select(s => new { s.InstituteId, s.CampusId })
            .FirstOrDefaultAsync();
        if (staff?.InstituteId != null)
            return (staff.InstituteId, staff.CampusId ?? campusId);

        // 2) Teacher class/subject assignment
        var fromTeaching = await _db.ClassSubjects.AsNoTracking().IgnoreQueryFilters()
            .Where(cs => cs.TeacherId == userId && !cs.IsDeleted && cs.InstituteId != null)
            .Select(cs => new { cs.InstituteId, cs.CampusId })
            .FirstOrDefaultAsync();
        if (fromTeaching?.InstituteId != null)
            return (fromTeaching.InstituteId, fromTeaching.CampusId ?? campusId);

        // Also try Class.InstituteId when ClassSubject row was not stamped
        var fromClass = await _db.ClassSubjects.AsNoTracking().IgnoreQueryFilters()
            .Where(cs => cs.TeacherId == userId && !cs.IsDeleted && cs.Class.InstituteId != null)
            .Select(cs => new { cs.Class.InstituteId, cs.Class.CampusId })
            .FirstOrDefaultAsync();
        if (fromClass?.InstituteId != null)
            return (fromClass.InstituteId, fromClass.CampusId ?? campusId);

        // 3) Campus FK on the user
        if (campusId.HasValue)
        {
            var fromCampus = await _db.Campuses.AsNoTracking().IgnoreQueryFilters()
                .Where(c => c.CampusId == campusId.Value)
                .Select(c => (int?)c.InstituteId)
                .FirstOrDefaultAsync();
            if (fromCampus.HasValue)
                return (fromCampus, campusId);
        }

        var profile = await _db.Users.AsNoTracking().IgnoreQueryFilters()
            .Where(u => u.UserId == userId)
            .Select(u => new { u.CreatedBy, u.Email })
            .FirstOrDefaultAsync();

        // 4) Creator's tenant (admin who created this login)
        if (profile?.CreatedBy is int createdBy)
        {
            var creator = await _db.Users.AsNoTracking().IgnoreQueryFilters()
                .Where(u => u.UserId == createdBy && u.InstituteId != null)
                .Select(u => new { u.InstituteId, u.CampusId })
                .FirstOrDefaultAsync();
            if (creator?.InstituteId != null)
                return (creator.InstituteId, creator.CampusId ?? campusId);
        }

        // 5) Same email domain as another user who already has a tenant
        var email = profile?.Email ?? "";
        var at = email.LastIndexOf('@');
        if (at > 0 && at < email.Length - 1)
        {
            var domain = email[(at + 1)..].ToLowerInvariant();
            var peer = await _db.Users.AsNoTracking().IgnoreQueryFilters()
                .Where(u => u.UserId != userId
                         && !u.IsDeleted
                         && u.InstituteId != null
                         && u.Email != null
                         && u.Email.ToLower().EndsWith("@" + domain))
                .Select(u => new { u.InstituteId, u.CampusId })
                .FirstOrDefaultAsync();
            if (peer?.InstituteId != null)
                return (peer.InstituteId, peer.CampusId ?? campusId);

            // Institute contact email on the same domain
            var byInstEmail = await _db.Institutes.AsNoTracking().IgnoreQueryFilters()
                .Where(i => i.IsActive && i.Email != null && i.Email.ToLower().EndsWith("@" + domain))
                .Select(i => (int?)i.InstituteId)
                .FirstOrDefaultAsync();
            if (byInstEmail.HasValue)
                return (byInstEmail, campusId);
        }

        return (null, campusId);
    }

    private async Task HealUserAsync(int userId, int instituteId, int? campusId)
    {
        var user = await _db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted);
        if (user == null) return;
        if (user.InstituteId == instituteId && (!campusId.HasValue || user.CampusId == campusId))
            return;

        user.InstituteId = instituteId;
        if (campusId.HasValue && user.CampusId == null)
            user.CampusId = campusId;
        user.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
