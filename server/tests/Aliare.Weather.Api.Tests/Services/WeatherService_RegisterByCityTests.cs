using Moq;
using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Domain.Models;
using Aliare.Weather.Api.Services;

namespace Aliare.Weather.Api.Tests.Services;

public class WeatherService_RegisterByCityTests
{
    [Fact]
    public async Task RegisterByCityAsync_WithValidCity_ShouldSaveRecord()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        mockProvider
            .Setup(p => p.GetByCityAsync("Cascavel", null, null))
            .ReturnsAsync(new WeatherResponse("Cascavel", 25.0, 90, 180));

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        WeatherRecord result = await service.RegisterByCityAsync("Cascavel");

        Assert.NotNull(result);
        Assert.Equal("Cascavel", result.CityName);
        Assert.Equal(25.0, result.Temperature);
        mockRepository.Verify(r => r.AddAsync(It.IsAny<WeatherRecord>()), Times.Once);
    }

    [Fact]
    public async Task RegisterByCityAsync_WithEmptyCity_ShouldThrowException()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        await Assert.ThrowsAsync<ArgumentException>(() => service.RegisterByCityAsync(string.Empty));
    }

    [Fact]
    public async Task RegisterByCityAsync_WhenProviderReturnsNull_ShouldThrowException()
    {
        Mock<IWeatherProvider> mockProvider = new();
        Mock<IWeatherRepository> mockRepository = new();

        mockProvider
            .Setup(p => p.GetByCityAsync("GhostTown", null, null))
            .ReturnsAsync((WeatherResponse?)null);

        WeatherService service = new(mockProvider.Object, mockRepository.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterByCityAsync("GhostTown"));
    }
}
