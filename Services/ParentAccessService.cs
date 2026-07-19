using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;

namespace SchoolManagement.API.Services;

/// <summary>
/// Central place to answer "which students belong to this parent" and
/// "is this parent allowed to see this student's data". Every controller
/// that exposes a parent-scoped endpoint (Attendance, Homework, Fees,
/// Students) should go through this instead of re-implementing the
/// StudentGuardians lookup, so the ownership rule only lives in one place.
/// </summary>
public interface IParentAccessService
{
    /// <summary>All StudentIds linked to this user via StudentGuardians.UserId.</summary>
    Task<List<int>> GetChildStudentIdsAsync(int guardianUserId);

    /// <summary>True if the given user is a non-deleted guardian of the given student.</summary>
    Task<bool> IsGuardianOfAsync(int guardianUserId, int studentId);

    /// <summary>True if any of this guardian's children has an active enrollment in the given class.</summary>
    Task<bool> HasChildInClassAsync(int guardianUserId, int classId);
}

public class ParentAccessService : IParentAccessService
{
    private readonly AppDbContext _db;
    public ParentAccessService(AppDbContext db) => _db = db;

    public async Task<List<int>> GetChildStudentIdsAsync(int guardianUserId)
        => await _db.StudentGuardians
            .Where(g => g.UserId == guardianUserId && !g.IsDeleted)
            .Select(g => g.StudentId)
            .Distinct()
            .ToListAsync();

    public async Task<bool> IsGuardianOfAsync(int guardianUserId, int studentId)
        => await _db.StudentGuardians
            .AnyAsync(g => g.UserId == guardianUserId && g.StudentId == studentId && !g.IsDeleted);

    public async Task<bool> HasChildInClassAsync(int guardianUserId, int classId)
        => await _db.StudentGuardians
            .Where(g => g.UserId == guardianUserId && !g.IsDeleted)
            .AnyAsync(g => g.Student.Enrollments.Any(e =>
                e.ClassId == classId && e.Status == "Active" && !e.IsDeleted));
}
