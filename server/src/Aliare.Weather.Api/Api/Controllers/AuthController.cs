using System.Security.Claims;
using Aliare.Weather.Api.Api.DTOs;
using Aliare.Weather.Api.Services;
using Aliare.Weather.Api.Services.Models;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aliare.Weather.Api.Api.Controllers;

[ApiVersion("1.0")]
public class AuthController(AuthService authService) : BaseApiController
{
    private const string RefreshTokenCookieName = "refresh_token";

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        try
        {
            await authService.RegisterAsync(request.Username, request.Email, request.Password);
            return Ok(new { message = "User registered successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            AuthResult result = await authService.LoginAsync(request.Email, request.Password);
            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(new AuthResponse(result.AccessToken));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        string? refreshToken = Request.Cookies[RefreshTokenCookieName];

        if (string.IsNullOrEmpty(refreshToken))
            return Unauthorized(new { error = "Refresh token not provided." });

        try
        {
            AuthResult result = await authService.RefreshAsync(refreshToken);
            SetRefreshTokenCookie(result.RefreshToken);
            return Ok(new AuthResponse(result.AccessToken));
        }
        catch (InvalidOperationException ex)
        {
            DeleteRefreshTokenCookie();
            return Unauthorized(new { error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized(new { error = "Invalid token." });

        try
        {
            await authService.RevokeRefreshTokenAsync(userId);
            DeleteRefreshTokenCookie();
            return Ok(new { message = "Logged out successfully." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private CookieOptions CreateCookieOptions(DateTimeOffset expires)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/api/v1/auth",
            Expires = expires
        };
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        CookieOptions cookieOptions = CreateCookieOptions(DateTimeOffset.UtcNow.AddDays(7));
        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    private void DeleteRefreshTokenCookie()
    {
        CookieOptions cookieOptions = CreateCookieOptions(DateTimeOffset.UtcNow.AddDays(-1));
        Response.Cookies.Append(RefreshTokenCookieName, string.Empty, cookieOptions);
    }
}
