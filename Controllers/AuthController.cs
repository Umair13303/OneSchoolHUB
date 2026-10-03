using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.API.DTOs.Auth;
using SchoolManagement.API.Services;

using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly AppDbContext _db;

    public AuthController(IAuthService authService, AppDbContext db)
    {
        _authService = authService;
        _db = db;
    }

    /// <summary>Temporary production diagnostics for login outage — remove after fix.</summary>
    [HttpGet("diag")]
    [AllowAnonymous]
    public async Task<IActionResult> Diag()
    {
        try
        {
            await _db.Database.OpenConnectionAsync();
            int hasPhotoCol;
            await using (var cmd = _db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT CASE WHEN COL_LENGTH('dbo.Users','PhotoFileId') IS NULL THEN 0 ELSE 1 END";
                hasPhotoCol = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }

            string? userQueryError = null;
            int sampleCount = -1;
            try
            {
                sampleCount = await _db.Users.IgnoreQueryFilters().AsNoTracking()
                    .Select(u => u.UserId)
                    .Take(1)
                    .CountAsync();
            }
            catch (Exception qx)
            {
                userQueryError = qx.InnerException?.Message ?? qx.Message;
            }

            return Ok(new
            {
                hasPhotoFileIdColumn = hasPhotoCol == 1,
                canQueryUserId = userQueryError == null,
                sampleCount,
                userQueryError
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
        }
    }

    /// <summary>Login with email and password</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var result = await _authService.LoginAsync(request, HttpContext);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            // Temporary: surface root cause while diagnosing production login 500s.
            return StatusCode(500, new
            {
                message = "Login failed.",
                error = ex.Message,
                inner = ex.InnerException?.Message
            });
        }
    }

    /// <summary>Get new access token using refresh token</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Invalid or expired refresh token." });

        return Ok(result);
    }

    /// <summary>Logout — revoke refresh token</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        await _authService.RevokeTokenAsync(refreshToken, HttpContext);
        return Ok(new { message = "Logged out successfully." });
    }
}
