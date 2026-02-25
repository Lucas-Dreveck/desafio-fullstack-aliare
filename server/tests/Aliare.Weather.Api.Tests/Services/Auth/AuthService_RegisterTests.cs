using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Infrastructure.Options;
using Aliare.Weather.Api.Services;
using Microsoft.Extensions.Options;

namespace Aliare.Weather.Api.Tests.Services.Auth;

public class AuthService_RegisterTests
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
    public async Task RegisterAsync_WithValidData_ShouldCreateUser()
    {
        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        User result = await service.RegisterAsync("aliare", "aliare@test.com", "123456");

        Assert.Equal("aliare", result.Username);
        Assert.Equal("aliare@test.com", result.Email);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowException()
    {
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("existing@test.com"))
            .ReturnsAsync(new User("existing", "existing@test.com", "hashedpassword"));

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterAsync("aliare", "existing@test.com", "123456"));
    }

    [Fact]
    public async Task RegisterAsync_WithShortPassword_ShouldThrowException()
    {
        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync("aliare", "aliare@test.com", "123"));
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidEmail_ShouldThrowException()
    {
        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync("aliare", "invalid-email", "123456"));
    }

    [Fact]
    public async Task RegisterAsync_WithShortUsername_ShouldThrowException()
    {
        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.RegisterAsync("ab", "aliare@test.com", "123456"));
    }
}
