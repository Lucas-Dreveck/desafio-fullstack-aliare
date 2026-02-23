using Aliare.Weather.Api.Domain.Models;

namespace Aliare.Weather.Api.Domain.Interfaces;

public interface IWeatherProvider
{
    Task<WeatherResponse> GetByCityAsync(string cityName);
    Task<WeatherResponse> GetByCoordinatesAsync(double latitude, double longitude);
}
