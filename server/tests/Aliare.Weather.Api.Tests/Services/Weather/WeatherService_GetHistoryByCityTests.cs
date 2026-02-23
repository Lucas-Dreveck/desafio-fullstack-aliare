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
            new("Cascavel", 25.0, 90, 180, DateTime.UtcNow),
            new("Cascavel", 25.0, 90, 180, DateTime.UtcNow.AddDays(-1)),
            new("Cascavel", 26.0, 90, 180, DateTime.UtcNow.AddDays(-2)),
        ];

        mockRepository
            .Setup(r => r.GetByCityAsync("Cascavel", 30))
            .ReturnsAsync(mockData);

        WeatherService weatherService = new(mockProvider.Object, mockRepository.Object);

        IEnumerable<WeatherRecord> result = await weatherService.GetHistoryByCityAsync("Cascavel");

        Assert.Equal(mockData, result);
        Assert.True(result.First().RecordedAt >= result.Last().RecordedAt);
        mockRepository.Verify(r => r.GetByCityAsync("Cascavel", 30), Times.Once);
    }

    [Fact]
    public async Task GetHistoryByCityAsync_WithNoRecords_ShouldReturnEmptyList()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        mockRepository
            .Setup(r => r.GetByCityAsync("Cascavel", 30))
            .ReturnsAsync([]);

        WeatherService weatherService = new(mockProvider.Object, mockRepository.Object);

        IEnumerable<WeatherRecord> result = await weatherService.GetHistoryByCityAsync("Cascavel");

        Assert.Empty(result);
        mockRepository.Verify(r => r.GetByCityAsync("Cascavel", 30), Times.Once);
    }

    [Fact]
    public async Task GetHistoryByCityAsync_WithEmptyCity_ShouldThrowException()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => service.GetHistoryByCityAsync(string.Empty));
    }
}
