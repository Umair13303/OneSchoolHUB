using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;
using SchoolManagement.API.Services;

namespace SchoolManagement.API.Tests;

public class InstituteMembershipTests : IDisposable
{
    private sealed class FixedTenant : ITenantContext
    {
        public int? InstituteId { get; init; }
        public int? CampusId { get; init; }
        public bool IsSuperAdmin { get; init; }
    }

    private readonly AppDbContext _db;
    private readonly InstituteMembershipService _svc;

    public InstituteMembershipTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("Mem_" + Guid.NewGuid().ToString("N"))
            .Options;
        _db = new AppDbContext(options, new FixedTenant { IsSuperAdmin = true });
        Seed();
        _svc = new InstituteMembershipService(_db);
    }

    public void Dispose() => _db.Dispose();

    private void Seed()
    {
        _db.Institutes.Add(new Institute
        {
            InstituteId = 1,
            Name = "HAPPY DAY EDUCATIONAL INSTITUTE",
            IsActive = true,
            LogoUrl = "/logos/happy.jpg"
        });
        _db.Institutes.Add(new Institute
        {
            InstituteId = 2,
            Name = "Other School",
            IsActive = true
        });

        // Orphan school admin — no InstituteId, no Staff, no teaching rows
        _db.Users.Add(new User
        {
            UserId = 10,
            FullName = "Admin",
            Email = "ADMIN@HAPPYDAY.COM",
            PasswordHash = "x",
            RoleId = 2,
            InstituteId = null,
            IsActive = true
        });

        // Teacher already stamped to the school
        _db.Users.Add(new User
        {
            UserId = 11,
            FullName = "Teacher",
            Email = "teacher@gmail.com",
            PasswordHash = "x",
            RoleId = 3,
            InstituteId = 1,
            CreatedBy = 10,
            IsActive = true
        });

        _db.SaveChanges();
    }

    [Fact]
    public async Task OrphanAdmin_InfersFromEmailDomainMatchingInstituteName()
    {
        var m = await _svc.ResolveAsync(10, null, null, "admin", heal: true);

        Assert.NotNull(m);
        Assert.Equal(1, m!.InstituteId);
        Assert.Equal("HAPPY DAY EDUCATIONAL INSTITUTE", m.Institute.Name);
        Assert.Equal("/logos/happy.jpg", m.Institute.LogoUrl);

        var healed = await _db.Users.IgnoreQueryFilters()
            .Where(u => u.UserId == 10)
            .Select(u => u.InstituteId)
            .FirstAsync();
        Assert.Equal(1, healed);
    }

    [Fact]
    public async Task OrphanAdmin_InfersFromUsersTheyCreated()
    {
        // Different email domain so name-match path is skipped; CreatedBy path must win
        var orphan = await _db.Users.IgnoreQueryFilters().FirstAsync(u => u.UserId == 10);
        orphan.Email = "admin@school.local";
        orphan.InstituteId = null;
        await _db.SaveChangesAsync();

        var m = await _svc.ResolveAsync(10, null, null, "admin", heal: true);

        Assert.NotNull(m);
        Assert.Equal(1, m!.InstituteId);
    }

    [Fact]
    public async Task SuperAdmin_ReturnsNull()
    {
        var m = await _svc.ResolveAsync(1, null, null, "superadmin", heal: true);
        Assert.Null(m);
    }

    [Fact]
    public async Task ExistingInstituteId_LoadsInstitute()
    {
        var m = await _svc.ResolveAsync(11, 1, null, "teacher", heal: false);

        Assert.NotNull(m);
        Assert.Equal(1, m!.InstituteId);
        Assert.Equal("HAPPY DAY EDUCATIONAL INSTITUTE", m.Institute.Name);
    }
}
