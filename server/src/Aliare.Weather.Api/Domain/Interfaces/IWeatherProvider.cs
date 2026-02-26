using Aliare.Weather.Api.Domain.Models;

namespace Aliare.Weather.Api.Domain.Interfaces;

public interface IWeatherProvider
{
    Task<WeatherResponse?> GetByCityAsync(string city, string? state = null, string? country = null);
    Task<WeatherResponse?> GetByCoordinatesAsync(double latitude, double longitude);
}
