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

    public async Task<WeatherResponse?> GetByCityAsync(string city, string? state = null, string? country = null)
    {
        string query = city;
        if (!string.IsNullOrWhiteSpace(state))
            query += $",{state}";
        if (!string.IsNullOrWhiteSpace(country))
            query += $",{country}";

        string geoUrl = $"{_options.GeoUrl}/direct?q={Uri.EscapeDataString(query)}&limit=1&appid={_options.ApiKey}";
        HttpResponseMessage geoResponse = await httpClient.GetAsync(geoUrl);
        geoResponse.EnsureSuccessStatusCode();

        string geoJson = await geoResponse.Content.ReadAsStringAsync();
        JsonElement geoArray = JsonDocument.Parse(geoJson).RootElement;

        if (geoArray.GetArrayLength() == 0)
            return null;

        JsonElement location = geoArray[0];
        double lat = location.GetProperty("lat").GetDouble();
        double lon = location.GetProperty("lon").GetDouble();
        string? resolvedCountry = location.TryGetProperty("country", out JsonElement countryEl) ? countryEl.GetString() : null;
        string? resolvedState = location.TryGetProperty("state", out JsonElement stateEl) ? stateEl.GetString() : null;

        WeatherResponse? weatherData = await FetchWeatherAsync(lat, lon);
        if (weatherData is null)
            return null;

        return weatherData with { Country = resolvedCountry, State = resolvedState };
    }

    public async Task<WeatherResponse?> GetByCoordinatesAsync(double latitude, double longitude)
    {
        WeatherResponse? weatherData = await FetchWeatherAsync(latitude, longitude);
        if (weatherData is null)
            return null;

        (string? country, string? state) = await ReverseGeocodeAsync(weatherData.Latitude, weatherData.Longitude);

        return weatherData with { Country = country, State = state };
    }

    private async Task<WeatherResponse?> FetchWeatherAsync(double latitude, double longitude)
    {
        string url = string.Format(
            CultureInfo.InvariantCulture,
            "{0}/weather?lat={1}&lon={2}&appid={3}&units=metric",
            _options.BaseUrl, latitude, longitude, _options.ApiKey);

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync();
        JsonElement root = JsonDocument.Parse(json).RootElement;

        return new WeatherResponse(
            City: root.GetProperty("name").GetString()!,
            Temperature: root.GetProperty("main").GetProperty("temp").GetDouble(),
            Latitude: root.GetProperty("coord").GetProperty("lat").GetDouble(),
            Longitude: root.GetProperty("coord").GetProperty("lon").GetDouble(),
            Country: null,
            State: null
        );
    }

    private async Task<(string? Country, string? State)> ReverseGeocodeAsync(double latitude, double longitude)
    {
        string url = string.Format(
            CultureInfo.InvariantCulture,
            "{0}/reverse?lat={1}&lon={2}&limit=1&appid={3}",
            _options.GeoUrl, latitude, longitude, _options.ApiKey);

        HttpResponseMessage response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return (null, null);

        string json = await response.Content.ReadAsStringAsync();
        JsonElement array = JsonDocument.Parse(json).RootElement;

        if (array.GetArrayLength() == 0)
            return (null, null);

        JsonElement location = array[0];
        string? country = location.TryGetProperty("country", out JsonElement countryEl) ? countryEl.GetString() : null;
        string? state = location.TryGetProperty("state", out JsonElement stateEl) ? stateEl.GetString() : null;

        return (country, state);
    }
}