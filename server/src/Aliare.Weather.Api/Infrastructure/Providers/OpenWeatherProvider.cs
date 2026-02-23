using System.Globalization;
using System.Net;
using System.Text.Json;
using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Domain.Models;
using Aliare.Weather.Api.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Aliare.Weather.Api.Infrastructure.Providers;

public class OpenWeatherProvider(HttpClient httpClient, IOptions<OpenWeatherOptions> options) : IWeatherProvider
{
    private readonly OpenWeatherOptions _options = options.Value;

    public async Task<WeatherResponse?> GetByCityAsync(string cityName, string? stateCode = null, string? countryCode = null)
    {
        string query = cityName;
        if (!string.IsNullOrWhiteSpace(stateCode))
            query += $",{stateCode}";
        if (!string.IsNullOrWhiteSpace(countryCode))
            query += $",{countryCode}";

        var geoUrl = $"{_options.GeoUrl}/direct?q={Uri.EscapeDataString(query)}&limit=1&appid={_options.ApiKey}";
        var geoResponse = await httpClient.GetAsync(geoUrl);
        geoResponse.EnsureSuccessStatusCode();

        var geoJson = await geoResponse.Content.ReadAsStringAsync();
        var geoArray = JsonDocument.Parse(geoJson).RootElement;

        if (geoArray.GetArrayLength() == 0)
            return null;

        var location = geoArray[0];
        var lat = location.GetProperty("lat").GetDouble();
        var lon = location.GetProperty("lon").GetDouble();

        return await GetByCoordinatesAsync(lat, lon);
    }

    public async Task<WeatherResponse?> GetByCoordinatesAsync(double latitude, double longitude)
    {
        var url = string.Format(
            CultureInfo.InvariantCulture,
            "{0}/weather?lat={1}&lon={2}&appid={3}&units=metric",
            _options.BaseUrl, latitude, longitude, _options.ApiKey);

        var response = await httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var root = JsonDocument.Parse(json).RootElement;

        return new WeatherResponse(
            CityName: root.GetProperty("name").GetString()!,
            Temperature: root.GetProperty("main").GetProperty("temp").GetDouble(),
            Latitude: root.GetProperty("coord").GetProperty("lat").GetDouble(),
            Longitude: root.GetProperty("coord").GetProperty("lon").GetDouble()
        );
    }
}