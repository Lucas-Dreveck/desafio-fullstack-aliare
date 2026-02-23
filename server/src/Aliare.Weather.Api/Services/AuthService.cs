using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Aliare.Weather.Api.Services;

public class AuthService(IUserRepository userRepository, IOptions<JwtOptions> jwtOptions)
{
    private readonly JwtOptions _jwt = jwtOptions.Value;

    public async Task<User> RegisterAsync(string username, string email, string password)
    {
        User.ValidateUsername(username);
        User.ValidateEmail(email);

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new ArgumentException("Password must be at least 6 characters.");

        User? existing = await userRepository.GetByEmailAsync(email);
        if (existing is not null)
            throw new InvalidOperationException("Email already registered.");

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        User user = new(username, email, passwordHash);

        await userRepository.AddAsync(user);
        return user;
    }

    public async Task<string> LoginAsync(string email, string password)
    {
        User? user = await userRepository.GetByEmailAsync(email)
            ?? throw new InvalidOperationException("Invalid email or password.");

        bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!valid)
            throw new InvalidOperationException("Invalid email or password.");

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        ];

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_jwt.SecretKey));
        SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
