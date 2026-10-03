using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Auth;
using SchoolManagement.API.Helpers;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, HttpContext? httpContext = null);
    Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task RevokeTokenAsync(string refreshToken, HttpContext? httpContext = null);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly LoggingDbContext _logDb;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, LoggingDbContext logDb, JwtHelper jwtHelper, IConfiguration config)
    {
        _db = db;
        _logDb = logDb;
        _jwtHelper = jwtHelper;
        _config = config;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request, HttpContext? httpContext = null)
    {
        // Project only columns required for auth. Avoid selecting Users.PhotoFileId so login
        // still works if that column has not been applied on production yet.
        var row = await _db.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(u => u.Email == request.Email && u.IsActive && !u.IsDeleted)
            .Select(u => new
            {
                u.UserId,
                u.FullName,
                u.Email,
                u.PasswordHash,
                u.RoleId,
                u.InstituteId,
                u.CampusId,
                RoleName = u.Role.RoleName,
                // Only columns required for auth/license — avoid Institute.ModuleCurriculum etc.
                InstituteName = u.Institute != null ? u.Institute.Name : null,
                InstituteLogoUrl = u.Institute != null ? u.Institute.LogoUrl : null,
                LicenseValidUntil = u.Institute != null ? u.Institute.LicenseValidUntil : null
            })
            .FirstOrDefaultAsync();

        if (row == null || !BCrypt.Net.BCrypt.Verify(request.Password, row.PasswordHash))
            return null;

        Institute? institute = null;
        if (row.InstituteId != null)
        {
            institute = new Institute
            {
                InstituteId = row.InstituteId.Value,
                Name = row.InstituteName ?? string.Empty,
                LogoUrl = row.InstituteLogoUrl,
                LicenseValidUntil = row.LicenseValidUntil
            };
        }

        if (IsLicenseExpired(institute))
            throw new UnauthorizedAccessException("Your school's license has expired. Please contact the system administrator to renew it.");

        var user = ToAuthUser(row.UserId, row.FullName, row.Email, row.RoleId, row.RoleName,
            row.InstituteId, row.CampusId, institute);

        var ip = httpContext?.Connection?.RemoteIpAddress?.ToString();
        var ua = httpContext?.Request?.Headers["User-Agent"].ToString();

        _logDb.ActivityLogs.Add(new ActivityLog
        {
            UserId      = user.UserId,
            UserName    = user.FullName,
            UserEmail   = user.Email,
            UserRole    = user.Role.RoleName,
            Action      = "Login",
            EntityName  = "User",
            EntityId    = user.UserId.ToString(),
            IpAddress   = ip,
            UserAgent   = ua,
            Timestamp   = DateTime.UtcNow,
            InstituteId = user.InstituteId
        });
        await _logDb.SaveChangesAsync();

        return await GenerateTokensAsync(user);
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = _jwtHelper.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) return null;

        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                       ?? principal.FindFirst("sub");
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            return null;

        var storedToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken
                                   && t.UserId == userId
                                   && !t.IsRevoked
                                   && t.ExpiresAt > DateTime.UtcNow);

        if (storedToken == null) return null;

        // Revoke old refresh token
        storedToken.IsRevoked = true;
        await _db.SaveChangesAsync();

        var row = await _db.Users
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(u => u.UserId == userId && u.IsActive && !u.IsDeleted)
            .Select(u => new
            {
                u.UserId,
                u.FullName,
                u.Email,
                u.RoleId,
                u.InstituteId,
                u.CampusId,
                RoleName = u.Role.RoleName,
                InstituteName = u.Institute != null ? u.Institute.Name : null,
                InstituteLogoUrl = u.Institute != null ? u.Institute.LogoUrl : null,
                LicenseValidUntil = u.Institute != null ? u.Institute.LicenseValidUntil : null
            })
            .FirstOrDefaultAsync();

        Institute? institute = null;
        if (row?.InstituteId != null)
        {
            institute = new Institute
            {
                InstituteId = row.InstituteId.Value,
                Name = row.InstituteName ?? string.Empty,
                LogoUrl = row.InstituteLogoUrl,
                LicenseValidUntil = row.LicenseValidUntil
            };
        }

        if (row == null || IsLicenseExpired(institute)) return null;

        var user = ToAuthUser(row.UserId, row.FullName, row.Email, row.RoleId, row.RoleName,
            row.InstituteId, row.CampusId, institute);
        return await GenerateTokensAsync(user);
    }

    private static User ToAuthUser(
        int userId, string fullName, string email, int roleId, string roleName,
        int? instituteId, int? campusId, Institute? institute) =>
        new()
        {
            UserId = userId,
            FullName = fullName,
            Email = email,
            RoleId = roleId,
            InstituteId = instituteId,
            CampusId = campusId,
            Role = new Role { RoleId = roleId, RoleName = roleName },
            Institute = institute
        };

    // License is valid through the end of LicenseValidUntil's date; null means unlimited.
    private static bool IsLicenseExpired(Models.Institute? institute) =>
        institute?.LicenseValidUntil != null && institute.LicenseValidUntil.Value.Date < DateTime.UtcNow.Date;

    public async Task RevokeTokenAsync(string refreshToken, HttpContext? httpContext = null)
    {
        var token = await _db.RefreshTokens
            .Include(t => t.User).ThenInclude(u => u!.Role)
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked);

        if (token != null)
        {
            token.IsRevoked = true;

            if (token.User != null)
            {
                var ip = httpContext?.Connection?.RemoteIpAddress?.ToString();
                var ua = httpContext?.Request?.Headers["User-Agent"].ToString();
                _logDb.ActivityLogs.Add(new ActivityLog
                {
                    UserId      = token.User.UserId,
                    UserName    = token.User.FullName,
                    UserEmail   = token.User.Email,
                    UserRole    = token.User.Role?.RoleName ?? "",
                    Action      = "Logout",
                    EntityName  = "User",
                    EntityId    = token.User.UserId.ToString(),
                    IpAddress   = ip,
                    UserAgent   = ua,
                    Timestamp   = DateTime.UtcNow,
                    InstituteId = token.User.InstituteId
                });
                await _logDb.SaveChangesAsync();
            }

            await _db.SaveChangesAsync();
        }
    }

    private async Task<LoginResponseDto> GenerateTokensAsync(User user)
    {
        var accessToken = _jwtHelper.GenerateAccessToken(user);
        var refreshTokenStr = _jwtHelper.GenerateRefreshToken();
        var refreshExpiryDays = int.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "7");
        var expiryMinutes = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "60");

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.UserId,
            Token = refreshTokenStr,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays)
        });

        await _db.SaveChangesAsync();

        string instituteName, tagline, logoUrl, copyrightText;

        if (user.Institute != null)
        {
            instituteName  = user.Institute.Name;
            tagline        = user.Institute.Name; // institutes don't have their own tagline field yet
            logoUrl        = user.Institute.LogoUrl ?? string.Empty;
            copyrightText  = string.Empty;
        }
        else
        {
            // superadmin — pull from DevCompany table
            var dev = await _db.DevCompany.AsNoTracking().FirstOrDefaultAsync();
            instituteName  = dev?.Name          ?? "Dev_Solutions";
            tagline        = dev?.Tagline        ?? string.Empty;
            logoUrl        = dev?.LogoUrl        ?? string.Empty;
            copyrightText  = dev?.CopyrightText  ?? string.Empty;
        }

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenStr,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            User = new UserInfoDto
            {
                UserId        = user.UserId,
                FullName      = user.FullName,
                Email         = user.Email,
                Role          = user.Role.RoleName,
                RoleId        = user.RoleId,
                InstituteId   = user.InstituteId,
                CampusId      = user.CampusId,
                InstituteName = instituteName,
                Tagline       = tagline,
                LogoUrl       = string.IsNullOrEmpty(logoUrl) ? null : logoUrl,
                CopyrightText = string.IsNullOrEmpty(copyrightText) ? null : copyrightText,
                PhotoFileId   = user.PhotoFileId
            }
        };
    }
}
