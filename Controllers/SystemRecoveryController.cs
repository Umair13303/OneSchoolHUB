using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;

namespace SchoolManagement.API.Controllers;

/// <summary>
/// Temporary recovery tools after tenant-heal hid null-InstituteId school data.
/// Protect with ?key= — remove once data is confirmed restored.
/// </summary>
[ApiController]
[Route("api/system-recovery")]
[AllowAnonymous]
public class SystemRecoveryController : ControllerBase
{
    private const string RecoveryKey = "recover-20261031";
    private readonly AppDbContext _db;

    public SystemRecoveryController(AppDbContext db) => _db = db;

    private bool Valid(string? key) =>
        string.Equals(key, RecoveryKey, StringComparison.Ordinal);

    [HttpGet("audit")]
    public async Task<IActionResult> Audit([FromQuery] string? key)
    {
        if (!Valid(key)) return Unauthorized();

        var institutes = await _db.Institutes.IgnoreQueryFilters().AsNoTracking()
            .Select(i => new { i.InstituteId, i.Name, i.IsActive, i.IsDeleted })
            .ToListAsync();

        var students = await _db.Students.IgnoreQueryFilters().AsNoTracking()
            .GroupBy(s => new { s.InstituteId, s.IsDeleted })
            .Select(g => new { g.Key.InstituteId, g.Key.IsDeleted, Count = g.Count() })
            .ToListAsync();

        var teachers = await _db.Users.IgnoreQueryFilters().AsNoTracking()
            .Where(u => u.Role.RoleName == "teacher")
            .GroupBy(u => new { u.InstituteId, u.IsDeleted, u.IsActive })
            .Select(g => new { g.Key.InstituteId, g.Key.IsDeleted, g.Key.IsActive, Count = g.Count() })
            .ToListAsync();

        var admins = await _db.Users.IgnoreQueryFilters().AsNoTracking()
            .Where(u => u.Role.RoleName == "admin")
            .Select(u => new { u.UserId, u.Email, u.FullName, u.InstituteId, u.IsDeleted, u.IsActive })
            .ToListAsync();

        var classes = await _db.Classes.IgnoreQueryFilters().AsNoTracking()
            .GroupBy(c => new { c.InstituteId, c.IsDeleted })
            .Select(g => new { g.Key.InstituteId, g.Key.IsDeleted, Count = g.Count() })
            .ToListAsync();

        return Ok(new { institutes, students, teachers, admins, classes });
    }

    /// <summary>
    /// Stamp/reassign school rows onto Happy Day (or ?instituteId=) so admin sees them again.
    /// Does not delete rows.
    /// </summary>
    [HttpPost("repair-happy-day")]
    public async Task<IActionResult> Repair([FromQuery] string? key, [FromQuery] int? instituteId = null)
    {
        if (!Valid(key)) return Unauthorized();

        var target = instituteId;
        if (!target.HasValue)
        {
            target = await _db.Institutes.IgnoreQueryFilters().AsNoTracking()
                .Where(i => !i.IsDeleted && i.IsActive
                    && i.Name.Replace(" ", "").ToLower().Contains("happyday"))
                .Select(i => (int?)i.InstituteId)
                .FirstOrDefaultAsync();
        }

        if (!target.HasValue)
        {
            target = await _db.Users.IgnoreQueryFilters().AsNoTracking()
                .Where(u => !u.IsDeleted && u.InstituteId != null
                    && u.Email != null && u.Email.ToLower().Contains("@happyday."))
                .Select(u => u.InstituteId)
                .FirstOrDefaultAsync();
        }

        if (!target.HasValue)
            return BadRequest(new { error = "Could not resolve Happy Day instituteId." });

        var campusId = await _db.Campuses.IgnoreQueryFilters().AsNoTracking()
            .Where(c => c.InstituteId == target && !c.IsDeleted)
            .Select(c => (int?)c.CampusId)
            .FirstOrDefaultAsync();

        var superAdminRoleId = await _db.Roles.AsNoTracking()
            .Where(r => r.RoleName == "superadmin")
            .Select(r => (int?)r.RoleId)
            .FirstOrDefaultAsync();

        // Prefer moving NULL-tenant orphans. If Happy Day still has almost no students,
        // also pull active rows from other institutes that have zero admins (legacy junk tenants).
        var happyStudentCount = await _db.Students.IgnoreQueryFilters()
            .CountAsync(s => !s.IsDeleted && s.InstituteId == target);

        var orphanInstitutes = await _db.Institutes.IgnoreQueryFilters().AsNoTracking()
            .Where(i => i.InstituteId != target && !i.IsDeleted)
            .Select(i => i.InstituteId)
            .ToListAsync();

        var emptyTenantIds = new List<int>();
        foreach (var id in orphanInstitutes)
        {
            var hasAdmin = await _db.Users.IgnoreQueryFilters().AsNoTracking()
                .AnyAsync(u => !u.IsDeleted && u.InstituteId == id && u.Role.RoleName == "admin");
            if (!hasAdmin) emptyTenantIds.Add(id);
        }

        async Task<int> StampStudentsAsync()
        {
            var q = _db.Students.IgnoreQueryFilters()
                .Where(s => !s.IsDeleted && (
                    s.InstituteId == null
                    || (happyStudentCount < 20 && emptyTenantIds.Contains(s.InstituteId!.Value))));
            var list = await q.ToListAsync();
            foreach (var s in list)
            {
                s.InstituteId = target;
                if (s.CampusId == null && campusId.HasValue) s.CampusId = campusId;
            }
            return list.Count;
        }

        async Task<int> StampUsersAsync()
        {
            var q = _db.Users.IgnoreQueryFilters()
                .Where(u => !u.IsDeleted
                    && (superAdminRoleId == null || u.RoleId != superAdminRoleId)
                    && (u.InstituteId == null
                        || (happyStudentCount < 20 && u.InstituteId != null && emptyTenantIds.Contains(u.InstituteId.Value))));
            var list = await q.ToListAsync();
            foreach (var u in list)
            {
                u.InstituteId = target;
                if (u.CampusId == null && campusId.HasValue) u.CampusId = campusId;
            }
            return list.Count;
        }

        async Task<int> StampClassesAsync()
        {
            var q = _db.Classes.IgnoreQueryFilters()
                .Where(c => !c.IsDeleted && (
                    c.InstituteId == null
                    || (happyStudentCount < 20 && emptyTenantIds.Contains(c.InstituteId!.Value))));
            var list = await q.ToListAsync();
            foreach (var c in list)
            {
                c.InstituteId = target;
                if (c.CampusId == null && campusId.HasValue) c.CampusId = campusId;
            }
            return list.Count;
        }

        async Task<int> StampSubjectsAsync()
        {
            var q = _db.Subjects.IgnoreQueryFilters()
                .Where(s => !s.IsDeleted && (
                    s.InstituteId == null
                    || (happyStudentCount < 20 && emptyTenantIds.Contains(s.InstituteId!.Value))));
            var list = await q.ToListAsync();
            foreach (var s in list)
            {
                s.InstituteId = target;
                if (s.CampusId == null && campusId.HasValue) s.CampusId = campusId;
            }
            return list.Count;
        }

        async Task<int> StampClassSubjectsAsync()
        {
            var q = _db.ClassSubjects.IgnoreQueryFilters()
                .Where(cs => !cs.IsDeleted && (
                    cs.InstituteId == null
                    || (happyStudentCount < 20 && emptyTenantIds.Contains(cs.InstituteId!.Value))));
            var list = await q.ToListAsync();
            foreach (var cs in list)
            {
                cs.InstituteId = target;
                if (cs.CampusId == null && campusId.HasValue) cs.CampusId = campusId;
            }
            return list.Count;
        }

        async Task<int> StampStaffAsync()
        {
            var q = _db.Staff.IgnoreQueryFilters()
                .Where(s => !s.IsDeleted && (
                    s.InstituteId == null
                    || (happyStudentCount < 20 && emptyTenantIds.Contains(s.InstituteId!.Value))));
            var list = await q.ToListAsync();
            foreach (var s in list)
            {
                s.InstituteId = target;
                if (s.CampusId == null && campusId.HasValue) s.CampusId = campusId;
            }
            return list.Count;
        }

        async Task<int> StampEnrollmentsAsync()
        {
            var q = _db.StudentClassEnrollments.IgnoreQueryFilters()
                .Where(e => !e.IsDeleted && (
                    e.InstituteId == null
                    || (happyStudentCount < 20 && emptyTenantIds.Contains(e.InstituteId!.Value))));
            var list = await q.ToListAsync();
            foreach (var e in list)
            {
                e.InstituteId = target;
                if (e.CampusId == null && campusId.HasValue) e.CampusId = campusId;
            }
            return list.Count;
        }

        var students = await StampStudentsAsync();
        var users = await StampUsersAsync();
        var classes = await StampClassesAsync();
        var subjects = await StampSubjectsAsync();
        var classSubjects = await StampClassSubjectsAsync();
        var staff = await StampStaffAsync();
        var enrollments = await StampEnrollmentsAsync();
        await _db.SaveChangesAsync();

        var afterStudents = await _db.Students.IgnoreQueryFilters()
            .CountAsync(s => !s.IsDeleted && s.InstituteId == target);
        var afterTeachers = await _db.Users.IgnoreQueryFilters()
            .CountAsync(u => !u.IsDeleted && u.IsActive && u.InstituteId == target && u.Role.RoleName == "teacher");

        return Ok(new
        {
            targetInstituteId = target,
            campusId,
            updated = new { students, users, classes, subjects, classSubjects, staff, enrollments },
            after = new { students = afterStudents, teachers = afterTeachers },
            emptyTenantIds
        });
    }
}
