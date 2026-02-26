using Aliare.Weather.Api.Domain.Models;
using Aliare.Weather.Api.Infrastructure.Providers;

namespace Aliare.Weather.Api.Tests.Services.Weather;

public class FakeWeatherProviderTests
{
    private readonly FakeWeatherProvider _provider = new();

    [Fact]
    public async Task GetByCityAsync_WithKnownCity_ShouldReturnResponse()
    {
        WeatherResponse? result = await _provider.GetByCityAsync("Cascavel");

        Assert.NotNull(result);
        Assert.Equal("Cascavel", result.City);
        Assert.Equal("Paraná", result.State);
        Assert.Equal("BR", result.Country);
    }

    [Fact]
    public async Task GetByCityAsync_WithUnknownCity_ShouldReturnNull()
    {
        WeatherResponse? result = await _provider.GetByCityAsync("GhostTown");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCityAsync_ShouldBeCaseInsensitive()
    {
        WeatherResponse? result = await _provider.GetByCityAsync("cascavel");

        Assert.NotNull(result);
        Assert.Equal("Cascavel", result.City);
    }

    [Fact]
    public async Task GetByCoordinatesAsync_ShouldReturnClosestCity()
    {
        WeatherResponse? result = await _provider.GetByCoordinatesAsync(-24.95, -53.45);

        Assert.NotNull(result);
        Assert.Equal("Cascavel", result.City);
        Assert.Equal("Paraná", result.State);
        Assert.Equal("BR", result.Country);
    }
}
