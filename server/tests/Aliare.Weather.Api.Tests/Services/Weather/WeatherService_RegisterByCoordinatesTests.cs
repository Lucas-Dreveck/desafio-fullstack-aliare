using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Domain.Models;
using Aliare.Weather.Api.Services;

namespace Aliare.Weather.Api.Tests.Services.Weather;

public class WeatherService_RegisterByCoordinatesTests
{
    [Fact]
    public async Task RegisterByCoordinatesAsync_WithValidCoordinates_ShouldSaveRecord()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        mockProvider
            .Setup(p => p.GetByCoordinatesAsync(90, 180))
            .ReturnsAsync(new WeatherResponse("Cascavel", "Paraná", "BR", 25.0, 90, 180));

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        WeatherRecord result = await service.RegisterByCoordinatesAsync(90, 180);

        Assert.NotNull(result);
        Assert.Equal("Cascavel", result.City);
        Assert.Equal("Paraná", result.State);
        Assert.Equal("BR", result.Country);
        Assert.Equal(90, result.Latitude);
        Assert.Equal(180, result.Longitude);
        Assert.Equal(25.0, result.Temperature);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<WeatherRecord>()), Times.Once);
    }

    [Fact]
    public async Task RegisterByCoordinatesAsync_WithInvalidCoordinates_ShouldThrowException()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.RegisterByCoordinatesAsync(180, 90));
    }
}
