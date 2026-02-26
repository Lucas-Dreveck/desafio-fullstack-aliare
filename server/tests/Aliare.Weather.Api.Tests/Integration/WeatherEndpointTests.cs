using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Aliare.Weather.Api.Api.DTOs;

namespace Aliare.Weather.Api.Tests.Integration;

public class WeatherEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public WeatherEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RegisterByCity_WithoutToken_ShouldReturnUnauthorized()
    {
        RegisterByCityRequest request = new("Cascavel");

        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/weather/register/by-city", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RegisterByCity_WithToken_ShouldReturnOk()
    {
        string token = await GetTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        RegisterByCityRequest request = new("Cascavel");
        HttpResponseMessage response = await _client.PostAsJsonAsync("/api/v1/weather/register/by-city", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        WeatherRecordResponse? body = await response.Content.ReadFromJsonAsync<WeatherRecordResponse>();
        Assert.NotNull(body);
        Assert.Equal("Cascavel", body.City);
        Assert.Equal("Paraná", body.State);
        Assert.Equal("BR", body.Country);
        Assert.Equal(25.0, body.Temperature);
    }

    [Fact]
    public async Task GetHistoryByCity_WithoutToken_ShouldReturnOk()
    {
        HttpResponseMessage response = await _client.GetAsync("/api/v1/weather/history/by-city/Cascavel");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private async Task<string> GetTokenAsync()
    {
        RegisterUserRequest registerRequest = new("aliare", $"weather-{Guid.NewGuid()}@test.com", "123456");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        LoginRequest loginRequest = new(registerRequest.Email, "123456");
        HttpResponseMessage loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        AuthResponse? auth = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return auth!.Token;
    }
}
