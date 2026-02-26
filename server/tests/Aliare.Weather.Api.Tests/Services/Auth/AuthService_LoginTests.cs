using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Infrastructure.Options;
using Aliare.Weather.Api.Services;
using Aliare.Weather.Api.Services.Models;
using Microsoft.Extensions.Options;

namespace Aliare.Weather.Api.Tests.Services.Auth;

public class AuthService_LoginTests
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly IOptions<JwtOptions> _jwtOptions = Options.Create(new JwtOptions
    {
        SecretKey = "TestSecretKeyThatIsAtLeast32Characters!",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpirationInMinutes = 15,
        RefreshTokenExpirationInDays = 7
    });

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("aliare@test.com"))
            .ReturnsAsync(new User("aliare", "aliare@test.com", passwordHash));

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        AuthResult result = await service.LoginAsync("aliare@test.com", "123456");

        Assert.NotNull(result.AccessToken);
        Assert.NotEmpty(result.AccessToken);
        Assert.Contains(".", result.AccessToken);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnTokenWithGuidSubClaim()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        User user = new("aliare", "aliare@test.com", passwordHash);
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("aliare@test.com"))
            .ReturnsAsync(user);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        AuthResult result = await service.LoginAsync("aliare@test.com", "123456");

        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwt = handler.ReadJwtToken(result.AccessToken);
        string? sub = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        Assert.NotNull(sub);
        Assert.True(Guid.TryParse(sub, out Guid parsedGuid));
        Assert.Equal(user.Id, parsedGuid);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ShouldThrowException()
    {
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("ghost@test.com"))
            .ReturnsAsync((User?)null);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.LoginAsync("ghost@test.com", "123456"));
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ShouldThrowException()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("correct-password");
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("aliare@test.com"))
            .ReturnsAsync(new User("aliare", "aliare@test.com", passwordHash));

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.LoginAsync("aliare@test.com", "wrong-password"));
    }
}
