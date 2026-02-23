using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Infrastructure.Options;
using Aliare.Weather.Api.Services;
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
        ExpirationInMinutes = 60
    });

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("aliare@test.com"))
            .ReturnsAsync(new User("aliare", "aliare@test.com", passwordHash));

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        string token = await service.LoginAsync("aliare@test.com", "123456");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
        Assert.Contains(".", token);
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
