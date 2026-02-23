using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Domain.Models;

namespace Aliare.Weather.Api.Infrastructure.Providers;

public class FakeWeatherProvider : IWeatherProvider
{
    private static readonly Dictionary<string, WeatherResponse> _cities = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Cascavel"] = new("Cascavel", 22.5, -24.9555, -53.4561),
        ["Toledo"] = new("Toledo", 24.0, -24.7246, -53.7430),
        ["Curitiba"] = new("Curitiba", 18.3, -25.4284, -49.2733),
        ["São Paulo"] = new("São Paulo", 26.1, -23.5505, -46.6340),
        ["Rio de Janeiro"] = new("Rio de Janeiro", 30.2, -22.9068, -43.1729)
    };

    public Task<WeatherResponse?> GetByCityAsync(string cityName, string? stateCode = null, string? countryCode = null)
    {
        _cities.TryGetValue(cityName, out WeatherResponse? response);
        return Task.FromResult(response);
    }

    public Task<WeatherResponse?> GetByCoordinatesAsync(double latitude, double longitude)
    {
        WeatherResponse? closest = _cities.Values
            .OrderBy(c => Math.Abs(c.Latitude - latitude) + Math.Abs(c.Longitude - longitude))
            .FirstOrDefault();

        return Task.FromResult(closest);
    }
}
