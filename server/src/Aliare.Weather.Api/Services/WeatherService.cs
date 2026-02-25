using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Domain.Models;

namespace Aliare.Weather.Api.Services;

public class WeatherService(IWeatherProvider weatherProvider, IWeatherRepository weatherRepository)
{
    public async Task<WeatherRecord> RegisterByCityAsync(string cityName, string? stateCode = null, string? countryCode = null)
    {
        WeatherRecord.ValidateCityName(cityName);
        WeatherResponse response = await weatherProvider.GetByCityAsync(cityName, stateCode, countryCode)
            ?? throw new InvalidOperationException($"Failed to retrieve weather data for city: {cityName}");

        WeatherRecord record = new WeatherRecord(
            response.CityName,
            response.Temperature,
            response.Latitude,
            response.Longitude,
            DateTime.UtcNow
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

        WeatherRecord record = new WeatherRecord(
            response.CityName,
            response.Temperature,
            response.Latitude,
            response.Longitude,
            DateTime.UtcNow
        );

        await weatherRepository.AddAsync(record);

        return record;
    }

    public async Task<IEnumerable<WeatherRecord>> GetHistoryByCityAsync(string cityName, int days = 30)
    {
        WeatherRecord.ValidateCityName(cityName);
        return await weatherRepository.GetByCityAsync(cityName, days);
    }

    public async Task<IEnumerable<WeatherRecord>> GetHistoryByCoordinatesAsync(double latitude, double longitude, int days = 30)
    {
        WeatherRecord.ValidateLatitude(latitude);
        WeatherRecord.ValidateLongitude(longitude);
        return await weatherRepository.GetByCoordinatesAsync(latitude, longitude, days);
    }
}
