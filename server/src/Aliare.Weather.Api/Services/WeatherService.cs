using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Domain.Models;

namespace Aliare.Weather.Api.Services;

public class WeatherService(IWeatherProvider weatherProvider, IWeatherRepository weatherRepository)
{
    public async Task<WeatherRecord> RegisterByCityAsync(string city, string? state = null, string? country = null)
    {
        WeatherRecord.ValidateCity(city);
        WeatherResponse response = await weatherProvider.GetByCityAsync(city, state, country)
            ?? throw new InvalidOperationException($"Failed to retrieve weather data for city: {city}");

        WeatherRecord record = new(
            response.City,
            response.Temperature,
            response.Latitude,
            response.Longitude,
            DateTime.UtcNow,
            response.State,
            response.Country
        );

        await weatherRepository.AddAsync(record);

        return record;
    }

    public async Task<WeatherRecord> RegisterByCoordinatesAsync(double latitude, double longitude)
    {
        WeatherRecord.ValidateLatitude(latitude);
        WeatherRecord.ValidateLongitude(longitude);
        WeatherResponse response = await weatherProvider.GetByCoordinatesAsync(latitude, longitude)
            ?? throw new InvalidOperationException($"Failed to retrieve weather data for coordinates: ({latitude}, {longitude})");

        WeatherRecord record = new(
            response.City,
            response.Temperature,
            response.Latitude,
            response.Longitude,
            DateTime.UtcNow,
            response.State,
            response.Country
        );

        await weatherRepository.AddAsync(record);

        return record;
    }

    public async Task<IEnumerable<WeatherRecord>> GetHistoryByCityAsync(string city, string? state = null, string? country = null, int days = 30)
    {
        WeatherRecord.ValidateCity(city);
        return await weatherRepository.GetByCityAsync(city, state, country, days);
    }

    public async Task<IEnumerable<WeatherRecord>> GetHistoryByCoordinatesAsync(double latitude, double longitude, int days = 30)
    {
        WeatherRecord.ValidateLatitude(latitude);
        WeatherRecord.ValidateLongitude(longitude);
        return await weatherRepository.GetByCoordinatesAsync(latitude, longitude, days);
    }
}
