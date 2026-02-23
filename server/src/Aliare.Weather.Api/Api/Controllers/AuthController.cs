using Aliare.Weather.Api.Api.DTOs;
using Aliare.Weather.Api.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Aliare.Weather.Api.Api.Controllers;

[ApiVersion("1.0")]
public class AuthController(AuthService authService) : BaseApiController
{
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
            string token = await authService.LoginAsync(request.Email, request.Password);
            return Ok(new AuthResponse(token));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
}
