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
        var user = await _db.Users
            .IgnoreQueryFilters()
            .Include(u => u.Role)
            .Include(u => u.Institute)
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive && !u.IsDeleted);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        if (IsLicenseExpired(user.Institute))
            throw new UnauthorizedAccessException("Your school's license has expired. Please contact the system administrator to renew it.");

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

        var user = await _db.Users
            .IgnoreQueryFilters()
            .Include(u => u.Role)
            .Include(u => u.Institute)
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive && !u.IsDeleted);

        if (user == null || IsLicenseExpired(user.Institute)) return null;

        return await GenerateTokensAsync(user);
    }

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
                CopyrightText = string.IsNullOrEmpty(copyrightText) ? null : copyrightText
            }
        };
    }
}
