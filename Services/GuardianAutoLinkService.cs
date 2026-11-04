using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

/// <summary>
/// Links StudentGuardians to school Users (teachers/staff/parents) by CNIC / phone / email
/// so a teacher whose child studies here can use the same login as a parent.
/// </summary>
public interface IGuardianAutoLinkService
{
    Task TryLinkGuardianAsync(StudentGuardian guardian);
    Task TryLinkUserToGuardiansAsync(User user);
}

public class GuardianAutoLinkService : IGuardianAutoLinkService
{
    private readonly AppDbContext _db;

    public GuardianAutoLinkService(AppDbContext db) => _db = db;

    public async Task TryLinkGuardianAsync(StudentGuardian guardian)
    {
        if (guardian.UserId.HasValue || guardian.IsDeleted) return;

        var student = await _db.Students.AsNoTracking()
            .Where(s => s.StudentId == guardian.StudentId)
            .Select(s => new { s.InstituteId })
            .FirstOrDefaultAsync();
        if (student?.InstituteId is null) return;

        var match = await FindMatchingUserAsync(student.InstituteId.Value, guardian.CNIC, guardian.Phone, null);
        if (match is null) return;

        guardian.UserId = match.UserId;
        await _db.SaveChangesAsync();
    }

    public async Task TryLinkUserToGuardiansAsync(User user)
    {
        if (user.InstituteId is null || user.IsDeleted || !user.IsActive) return;

        var cnicKey = NormalizeCnic(user.CNIC);
        var phoneKey = NormalizePhone(user.Phone);
        if (cnicKey is null && phoneKey is null && string.IsNullOrWhiteSpace(user.Email))
            return;

        var candidates = await _db.StudentGuardians
            .Include(g => g.Student)
            .Where(g => !g.IsDeleted
                     && g.UserId == null
                     && g.Student.InstituteId == user.InstituteId
                     && !g.Student.IsDeleted)
            .ToListAsync();

        var linked = false;
        foreach (var g in candidates)
        {
            if (Matches(g.CNIC, g.Phone, null, cnicKey, phoneKey, user.Email))
            {
                g.UserId = user.UserId;
                linked = true;
            }
        }

        if (linked)
            await _db.SaveChangesAsync();
    }

    private async Task<User?> FindMatchingUserAsync(int instituteId, string? cnic, string? phone, string? email)
    {
        var cnicKey = NormalizeCnic(cnic);
        var phoneKey = NormalizePhone(phone);

        var users = await _db.Users.AsNoTracking()
            .Where(u => !u.IsDeleted && u.IsActive && u.InstituteId == instituteId)
            .Select(u => new User
            {
                UserId = u.UserId,
                Email = u.Email,
                Phone = u.Phone,
                CNIC = u.CNIC,
                RoleId = u.RoleId
            })
            .ToListAsync();

        var matches = users.Where(u => Matches(cnic, phone, email, NormalizeCnic(u.CNIC), NormalizePhone(u.Phone), u.Email)).ToList();
        // Prefer unique match; if multiple, prefer parent then teacher.
        if (matches.Count == 0) return null;
        if (matches.Count == 1) return matches[0];
        return matches.OrderBy(u => u.RoleId == 5 ? 0 : u.RoleId == 4 ? 1 : 2).First();
    }

    private static bool Matches(string? gCnic, string? gPhone, string? gEmail,
        string? userCnicKey, string? userPhoneKey, string? userEmail)
    {
        var gCnicKey = NormalizeCnic(gCnic);
        if (gCnicKey is not null && userCnicKey is not null && gCnicKey == userCnicKey)
            return true;

        var gPhoneKey = NormalizePhone(gPhone);
        if (gPhoneKey is not null && userPhoneKey is not null && gPhoneKey == userPhoneKey)
            return true;

        if (!string.IsNullOrWhiteSpace(gEmail) && !string.IsNullOrWhiteSpace(userEmail)
            && string.Equals(gEmail.Trim(), userEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static string? NormalizeCnic(string? cnic)
    {
        if (string.IsNullOrWhiteSpace(cnic)) return null;
        var digits = new string(cnic.Where(char.IsDigit).ToArray());
        return digits.Length >= 5 ? digits : null;
    }

    private static string? NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return null;
        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.Length < 10) return null;
        return digits.Length > 10 ? digits[^10..] : digits;
    }
}
