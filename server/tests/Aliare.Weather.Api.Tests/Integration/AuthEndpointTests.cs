using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
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
    public async Task Login_WithValidCredentials_ShouldReturnTokenAndSetCookie()
    {
        RegisterUserRequest registerRequest = new("aliare", "login@test.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        LoginRequest loginRequest = new("login@test.com", "123456");
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        AuthResponse? body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Token);

        Assert.True(response.Headers.Contains("Set-Cookie"));
        string cookieHeader = response.Headers.GetValues("Set-Cookie").First();
        Assert.Contains("refresh_token=", cookieHeader);
        Assert.Contains("httponly", cookieHeader, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("path=/api/v1/auth", cookieHeader, StringComparison.OrdinalIgnoreCase);
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

    [Fact]
    public async Task Refresh_WithValidCookie_ShouldReturnNewTokenAndRotateCookie()
    {
        RegisterUserRequest registerRequest = new("aliare", "refresh@test.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        LoginRequest loginRequest = new("refresh@test.com", "123456");
        HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        string loginCookie = loginResponse.Headers.GetValues("Set-Cookie").First();

        HttpRequestMessage refreshRequest = new(HttpMethod.Post, "/api/v1/auth/refresh");
        refreshRequest.Headers.Add("Cookie", ExtractCookieValue(loginCookie));

        HttpResponseMessage refreshResponse = await _client.SendAsync(refreshRequest);

        Assert.Equal(HttpStatusCode.OK, refreshResponse.StatusCode);
        AuthResponse? body = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.NotEmpty(body.Token);

        Assert.True(refreshResponse.Headers.Contains("Set-Cookie"));
        string newCookie = refreshResponse.Headers.GetValues("Set-Cookie").First();
        Assert.Contains("refresh_token=", newCookie);
    }

    [Fact]
    public async Task Refresh_WithoutCookie_ShouldReturnUnauthorized()
    {
        HttpResponseMessage response = await _client.PostAsync("/api/v1/auth/refresh", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_WithValidToken_ShouldReturnOkAndClearCookie()
    {
        RegisterUserRequest registerRequest = new("aliare", "logout@test.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        LoginRequest loginRequest = new("logout@test.com", "123456");
        HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        AuthResponse? loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        string loginCookie = loginResponse.Headers.GetValues("Set-Cookie").First();

        HttpRequestMessage logoutRequest = new(HttpMethod.Post, "/api/v1/auth/logout");
        logoutRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginBody!.Token);
        logoutRequest.Headers.Add("Cookie", ExtractCookieValue(loginCookie));

        HttpResponseMessage logoutResponse = await _client.SendAsync(logoutRequest);

        Assert.Equal(HttpStatusCode.OK, logoutResponse.StatusCode);
    }

    [Fact]
    public async Task Refresh_AfterLogout_ShouldReturnUnauthorized()
    {
        RegisterUserRequest registerRequest = new("aliare", "revoked@test.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        LoginRequest loginRequest = new("revoked@test.com", "123456");
        HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        AuthResponse? loginBody = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        string loginCookie = loginResponse.Headers.GetValues("Set-Cookie").First();

        HttpRequestMessage logoutRequest = new(HttpMethod.Post, "/api/v1/auth/logout");
        logoutRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginBody!.Token);
        logoutRequest.Headers.Add("Cookie", ExtractCookieValue(loginCookie));
        await _client.SendAsync(logoutRequest);

        HttpRequestMessage refreshRequest = new(HttpMethod.Post, "/api/v1/auth/refresh");
        refreshRequest.Headers.Add("Cookie", ExtractCookieValue(loginCookie));

        HttpResponseMessage refreshResponse = await _client.SendAsync(refreshRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
    }

    private static string ExtractCookieValue(string setCookieHeader)
    {
        return setCookieHeader.Split(';')[0];
    }
}
