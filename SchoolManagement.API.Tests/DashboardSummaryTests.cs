using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Settings;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Tests;

/// <summary>
/// Parity: DashboardService aggregates must match client-side reduction of pageSize=0 list.
/// </summary>
public class DashboardSummaryTests : IDisposable
{
    private sealed class FixedTenant : ITenantContext
    {
        public int? InstituteId { get; init; }
        public int? CampusId { get; init; }
        public bool IsSuperAdmin { get; init; }
    }

    private readonly AppDbContext _db;
    private readonly StudentService _students;
    private readonly DashboardService _dashboard;

    public DashboardSummaryTests()
    {
        var tenant = new FixedTenant { InstituteId = 1, IsSuperAdmin = false };
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Dash_" + Guid.NewGuid().ToString("N"))
            .Options;
        _db = new AppDbContext(options, tenant);
        Seed();
        _students = new StudentService(
            _db,
            new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build(),
            new SettingsStub());
        _dashboard = new DashboardService(_db);
    }

    public void Dispose() => _db.Dispose();

    private void Seed()
    {
        _db.AcademicYears.Add(new AcademicYear
        {
            AcademicYearId = 1,
            YearLabel = "2026-27",
            StartDate = new DateOnly(2026, 4, 1),
            EndDate = new DateOnly(2027, 3, 31),
            IsActive = true,
            InstituteId = 1
        });
        _db.Classes.AddRange(
            new Class { ClassId = 1, ClassName = "Grade 1", Section = "A", IsActive = true, InstituteId = 1 },
            new Class { ClassId = 2, ClassName = "Grade 2", Section = null, IsActive = true, InstituteId = 1 },
            new Class { ClassId = 99, ClassName = "OtherSchool", IsActive = true, InstituteId = 2 }
        );

        // Tenant 1 students covering parity cases
        AddStudent(1, "Male", active: true, deleted: false, enrollments: new[]
        {
            (classId: 1, status: "Withdrawn", date: new DateOnly(2025, 1, 1)),
            (classId: 2, status: "Active", date: new DateOnly(2026, 1, 1)), // latest active → Grade 2
        });
        AddStudent(2, "FEMALE", active: true, deleted: false, enrollments: new[]
        {
            (classId: 1, status: "Active", date: new DateOnly(2026, 2, 1)),
        });
        AddStudent(3, "female", active: false, deleted: false, enrollments: new[]
        {
            (classId: 1, status: "Active", date: new DateOnly(2026, 3, 1)),
        }); // inactive still counted
        AddStudent(4, null, active: true, deleted: false, enrollments: Array.Empty<(int, string, DateOnly)>()); // Unknown
        AddStudent(5, "Other", active: true, deleted: false, enrollments: new[]
        {
            (classId: 1, status: "Active", date: new DateOnly(2026, 1, 1)),
        });
        AddStudent(6, "", active: true, deleted: false, enrollments: new[]
        {
            (classId: 1, status: "Promoted", date: new DateOnly(2026, 1, 1)), // no active → Unknown
        });
        AddStudent(7, "male", active: true, deleted: true, enrollments: Array.Empty<(int, string, DateOnly)>()); // excluded

        // Other institute — must not appear for tenant 1
        AddStudent(100, "Male", active: true, deleted: false, instituteId: 2, enrollments: new[]
        {
            (classId: 99, status: "Active", date: new DateOnly(2026, 1, 1)),
        });

        _db.SaveChanges();
    }

    private void AddStudent(
        int id,
        string? gender,
        bool active,
        bool deleted,
        (int classId, string status, DateOnly date)[] enrollments,
        int instituteId = 1)
    {
        _db.Students.Add(new Student
        {
            StudentId = id,
            AdmissionNo = $"ADM-{id}",
            FirstName = "S",
            LastName = id.ToString(),
            Gender = gender,
            IsActive = active,
            IsDeleted = deleted,
            InstituteId = instituteId,
            AdmissionDate = new DateOnly(2026, 1, 1)
        });
        var eid = id * 10;
        foreach (var (classId, status, date) in enrollments)
        {
            _db.StudentClassEnrollments.Add(new StudentClassEnrollment
            {
                EnrollmentId = ++eid,
                StudentId = id,
                ClassId = classId,
                AcademicYearId = 1,
                EnrollmentDate = date,
                Status = status,
                IsDeleted = false,
                InstituteId = instituteId
            });
        }
    }

    [Fact]
    public async Task Summary_Matches_PageSize0_ClientMath()
    {
        var roster = await _students.GetAllAsync(null, null, null, 1, 0);
        var items = roster.Items;

        var total = items.Count;
        var male = items.Count(s => s.Gender?.ToLower() == "male");
        var female = items.Count(s => s.Gender?.ToLower() == "female");
        var classCounts = items
            .GroupBy(s => string.IsNullOrEmpty(s.ClassName) ? "Unknown" : s.ClassName!)
            .ToDictionary(g => g.Key, g => g.Count());

        var summary = await _dashboard.GetSummaryAsync();

        Assert.Equal(total, summary.TotalStudents);
        Assert.Equal(male, summary.MaleCount);
        Assert.Equal(female, summary.FemaleCount);
        Assert.Equal(classCounts.Count, summary.ByClass.Count);
        foreach (var row in summary.ByClass)
            Assert.Equal(classCounts[row.Name], row.Count);

        // Absolute expectations for seeded tenant 1 (ids 1–6)
        Assert.Equal(6, summary.TotalStudents);
        Assert.Equal(1, summary.MaleCount);
        Assert.Equal(2, summary.FemaleCount);
        Assert.Equal(1, summary.ByClass.First(c => c.Name == "Grade 2").Count);
        Assert.Equal(3, summary.ByClass.First(c => c.Name == "Grade 1").Count);
        Assert.Equal(2, summary.ByClass.First(c => c.Name == "Unknown").Count);
    }

    [Fact]
    public async Task Summary_Excludes_Other_Institute()
    {
        var summary = await _dashboard.GetSummaryAsync();
        Assert.DoesNotContain(summary.ByClass, c => c.Name == "OtherSchool");
        Assert.Equal(6, summary.TotalStudents);
    }

    [Fact]
    public async Task Superadmin_Sees_All_Institutes()
    {
        var tenant = new FixedTenant { InstituteId = null, IsSuperAdmin = true };
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("DashSA_" + Guid.NewGuid().ToString("N"))
            .Options;
        await using var db = new AppDbContext(options, tenant);

        db.AcademicYears.Add(new AcademicYear
        {
            AcademicYearId = 1, YearLabel = "Y", StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31), IsActive = true, InstituteId = 1
        });
        db.Classes.AddRange(
            new Class { ClassId = 1, ClassName = "A", IsActive = true, InstituteId = 1 },
            new Class { ClassId = 2, ClassName = "B", IsActive = true, InstituteId = 2 });
        db.Students.AddRange(
            new Student { StudentId = 1, AdmissionNo = "1", FirstName = "A", LastName = "1", Gender = "Male", InstituteId = 1, IsActive = true },
            new Student { StudentId = 2, AdmissionNo = "2", FirstName = "B", LastName = "2", Gender = "Female", InstituteId = 2, IsActive = true });
        db.StudentClassEnrollments.AddRange(
            new StudentClassEnrollment { EnrollmentId = 1, StudentId = 1, ClassId = 1, AcademicYearId = 1, Status = "Active", EnrollmentDate = new DateOnly(2026, 1, 1), InstituteId = 1 },
            new StudentClassEnrollment { EnrollmentId = 2, StudentId = 2, ClassId = 2, AcademicYearId = 1, Status = "Active", EnrollmentDate = new DateOnly(2026, 1, 1), InstituteId = 2 });
        await db.SaveChangesAsync();

        var summary = await new DashboardService(db).GetSummaryAsync();
        Assert.Equal(2, summary.TotalStudents);
        Assert.Equal(1, summary.MaleCount);
        Assert.Equal(1, summary.FemaleCount);
    }

    private sealed class SettingsStub : ISettingsService
    {
        public Task<SchoolSettingsDto> GetAsync() => throw new NotImplementedException();
        public Task<SchoolSettingsDto> SaveAsync(UpdateSchoolSettingsDto dto, int updatedBy) => throw new NotImplementedException();
    }
}
