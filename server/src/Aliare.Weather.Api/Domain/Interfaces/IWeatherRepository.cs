using Aliare.Weather.Api.Domain.Entities;

namespace Aliare.Weather.Api.Domain.Interfaces;

public interface IWeatherRepository
{
    Task AddAsync(WeatherRecord record);
    Task<IEnumerable<WeatherRecord>> GetByCityAsync(string cityName, int days = 30);
    Task<IEnumerable<WeatherRecord>> GetByCoordinatesAsync(double latitude, double longitude, int days = 30);
}
