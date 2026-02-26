using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Services;

namespace Aliare.Weather.Api.Tests.Services.Weather;

public class WeatherService_GetHistoryByCityTests
{
    [Fact]
    public async Task GetHistoryByCityAsync_WithValidCity_ShouldReturnRecords()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        List<WeatherRecord> mockData =
        [
            new("Cascavel", 25.0, 90, 180, DateTime.UtcNow, "Paraná", "BR"),
            new("Cascavel", 25.0, 90, 180, DateTime.UtcNow.AddDays(-1), "Paraná", "BR"),
            new("Cascavel", 26.0, 90, 180, DateTime.UtcNow.AddDays(-2), "Paraná", "BR"),
        ];

        mockRepository
            .Setup(r => r.GetByCityAsync("Cascavel", null, null, 30))
            .ReturnsAsync(mockData);

        WeatherService weatherService = new(mockProvider.Object, mockRepository.Object);

        IEnumerable<WeatherRecord> result = await weatherService.GetHistoryByCityAsync("Cascavel");

        Assert.Equal(mockData, result);
        Assert.True(result.First().RecordedAt >= result.Last().RecordedAt);
        mockRepository.Verify(r => r.GetByCityAsync("Cascavel", null, null, 30), Times.Once);
    }

    [Fact]
    public async Task GetHistoryByCityAsync_WithNoRecords_ShouldReturnEmptyList()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        mockRepository
            .Setup(r => r.GetByCityAsync("Cascavel", null, null, 30))
            .ReturnsAsync([]);

        WeatherService weatherService = new(mockProvider.Object, mockRepository.Object);

        IEnumerable<WeatherRecord> result = await weatherService.GetHistoryByCityAsync("Cascavel");

        Assert.Empty(result);
        mockRepository.Verify(r => r.GetByCityAsync("Cascavel", null, null, 30), Times.Once);
    }

    [Fact]
    public async Task GetHistoryByCityAsync_WithEmptyCity_ShouldThrowException()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetHistoryByCityAsync(string.Empty));
    }

    [Fact]
    public async Task GetHistoryByCityAsync_WithCountryAndState_ShouldPassFilters()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        List<WeatherRecord> mockData =
        [
            new("Cascavel", 25.0, -24.9555, -53.4561, DateTime.UtcNow, "Paraná", "BR"),
        ];

        mockRepository
            .Setup(r => r.GetByCityAsync("Cascavel", "Paraná", "BR", 30))
            .ReturnsAsync(mockData);

        WeatherService weatherService = new(mockProvider.Object, mockRepository.Object);

        IEnumerable<WeatherRecord> result = await weatherService.GetHistoryByCityAsync("Cascavel", country: "BR", state: "Paraná");

        Assert.Single(result);
        Assert.Equal("BR", result.First().Country);
        Assert.Equal("Paraná", result.First().State);
        mockRepository.Verify(r => r.GetByCityAsync("Cascavel", "Paraná", "BR", 30), Times.Once);
    }
}
