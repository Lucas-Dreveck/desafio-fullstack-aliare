using Aliare.Weather.Api.Domain.Entities;

namespace Aliare.Weather.Api.Tests.Domain.Auth;

public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        User user = new("aliare", "aliare@test.com", "hashedpassword");

        Assert.Equal("aliare", user.Username);
        Assert.Equal("aliare@test.com", user.Email);
        Assert.Equal("hashedpassword", user.PasswordHash);
    }

    [Fact]
    public void Constructor_WithEmptyUsername_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new User("", "aliare@test.com", "hashedpassword"));
    }

    [Fact]
    public void Constructor_WithShortUsername_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new User("ab", "aliare@test.com", "hashedpassword"));
    }

    [Fact]
    public void Constructor_WithNullUsername_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => new User(null!, "aliare@test.com", "hashedpassword"));
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new User("aliare", "invalid-email", "hashedpassword"));
    }

    [Fact]
    public void Constructor_WithEmptyEmail_ShouldThrowException()
    {
        Assert.Throws<ArgumentException>(() => new User("aliare", "", "hashedpassword"));
    }

    [Fact]
    public void Constructor_WithNullEmail_ShouldThrowException()
    {
        Assert.Throws<ArgumentNullException>(() => new User("aliare", null!, "hashedpassword"));
    }
}
