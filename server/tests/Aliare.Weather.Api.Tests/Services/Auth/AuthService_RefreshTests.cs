using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Infrastructure.Options;
using Aliare.Weather.Api.Services;
using Aliare.Weather.Api.Services.Models;
using Microsoft.Extensions.Options;

namespace Aliare.Weather.Api.Tests.Services.Auth;

public class AuthService_RefreshTests
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
    public async Task LoginAsync_ShouldReturnAccessTokenAndRefreshToken()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        User user = new("aliare", "aliare@test.com", passwordHash);
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("aliare@test.com"))
            .ReturnsAsync(user);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        AuthResult result = await service.LoginAsync("aliare@test.com", "123456");

        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_ShouldSaveRefreshTokenOnUser()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        User user = new("aliare", "aliare@test.com", passwordHash);
        _mockUserRepository
            .Setup(r => r.GetByEmailAsync("aliare@test.com"))
            .ReturnsAsync(user);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        AuthResult result = await service.LoginAsync("aliare@test.com", "123456");

        Assert.Equal(result.RefreshToken, user.RefreshToken);
        Assert.NotNull(user.RefreshTokenExpiresAt);
        Assert.True(user.RefreshTokenExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task RefreshAsync_WithValidToken_ShouldReturnNewTokens()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        User user = new("aliare", "aliare@test.com", passwordHash);
        user.SetRefreshToken("valid-refresh-token", DateTime.UtcNow.AddDays(7));

        _mockUserRepository
            .Setup(r => r.GetByRefreshTokenAsync("valid-refresh-token"))
            .ReturnsAsync(user);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        AuthResult result = await service.RefreshAsync("valid-refresh-token");

        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.NotEqual("valid-refresh-token", result.RefreshToken);
    }

    [Fact]
    public async Task RefreshAsync_WithValidToken_ShouldRotateRefreshToken()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        User user = new("aliare", "aliare@test.com", passwordHash);
        user.SetRefreshToken("old-refresh-token", DateTime.UtcNow.AddDays(7));

        _mockUserRepository
            .Setup(r => r.GetByRefreshTokenAsync("old-refresh-token"))
            .ReturnsAsync(user);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        AuthResult result = await service.RefreshAsync("old-refresh-token");

        Assert.NotEqual("old-refresh-token", user.RefreshToken);
        Assert.Equal(result.RefreshToken, user.RefreshToken);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_WithNonExistentToken_ShouldThrowException()
    {
        _mockUserRepository
            .Setup(r => r.GetByRefreshTokenAsync("non-existent"))
            .ReturnsAsync((User?)null);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RefreshAsync("non-existent"));
    }

    [Fact]
    public async Task RefreshAsync_WithExpiredToken_ShouldThrowException()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        User user = new("aliare", "aliare@test.com", passwordHash);
        user.SetRefreshToken("expired-token", DateTime.UtcNow.AddDays(-1));

        _mockUserRepository
            .Setup(r => r.GetByRefreshTokenAsync("expired-token"))
            .ReturnsAsync(user);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RefreshAsync("expired-token"));
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_ShouldClearRefreshTokenOnUser()
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        User user = new("aliare", "aliare@test.com", passwordHash);
        user.SetRefreshToken("token-to-revoke", DateTime.UtcNow.AddDays(7));

        _mockUserRepository
            .Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await service.RevokeRefreshTokenAsync(user.Id);

        Assert.Null(user.RefreshToken);
        Assert.Null(user.RefreshTokenExpiresAt);
        _mockUserRepository.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [Fact]
    public async Task RevokeRefreshTokenAsync_WithNonExistentUser_ShouldThrowException()
    {
        _mockUserRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        AuthService service = new(_mockUserRepository.Object, _jwtOptions);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RevokeRefreshTokenAsync(Guid.NewGuid()));
    }
}
