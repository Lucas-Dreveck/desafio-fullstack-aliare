using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Services;

namespace Aliare.Weather.Api.Tests.Services.Weather;

public class WeatherService_GetHistoryByCoordinatesTests
{
    [Fact]
    public async Task GetHistoryByCoordinatesAsync_WithValidCoordinates_ShouldReturnRecords()
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
            .Setup(r => r.GetByCoordinatesAsync(90, 180, 30))
            .ReturnsAsync(mockData);

        WeatherService weatherService = new(mockProvider.Object, mockRepository.Object);

        IEnumerable<WeatherRecord> result = await weatherService.GetHistoryByCoordinatesAsync(90, 180);

        Assert.Equal(mockData, result);
        Assert.True(result.First().RecordedAt >= result.Last().RecordedAt);
        mockRepository.Verify(r => r.GetByCoordinatesAsync(90, 180, 30), Times.Once);
    }

    [Fact]
    public async Task GetHistoryByCoordinatesAsync_WithNoRecords_ShouldReturnEmptyList()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        mockRepository
            .Setup(r => r.GetByCoordinatesAsync(90, 180, 30))
            .ReturnsAsync([]);

        WeatherService weatherService = new(mockProvider.Object, mockRepository.Object);

        IEnumerable<WeatherRecord> result = await weatherService.GetHistoryByCoordinatesAsync(90, 180);

        Assert.Empty(result);
        mockRepository.Verify(r => r.GetByCoordinatesAsync(90, 180, 30), Times.Once);
    }

    [Fact]
    public async Task GetHistoryByCoordinatesAsync_WithInvalidCoordinates_ShouldThrowException()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.GetHistoryByCoordinatesAsync(180, 90));
    }
}
