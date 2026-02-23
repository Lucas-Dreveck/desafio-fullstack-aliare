using System.Net;
using System.Net.Http.Json;
using Aliare.Weather.Api.Api.DTOs;

namespace Aliare.Weather.Api.Tests.Integration;

public class AuthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnOk()
    {
        RegisterUserRequest request = new("aliare", "aliare@test.com", "123456");

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnConflict()
    {
        RegisterUserRequest request = new("aliare", "duplicate@test.com", "123456");

        await _client.PostAsJsonAsync("/api/v1/auth/register", request);
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        RegisterUserRequest registerRequest = new("aliare", "login@test.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        LoginRequest loginRequest = new("login@test.com", "123456");
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthResponse? body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Token);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ShouldReturnUnauthorized()
    {
        RegisterUserRequest registerRequest = new("aliare", "wrong@test.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        LoginRequest loginRequest = new("wrong@test.com", "wrongpassword");
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
