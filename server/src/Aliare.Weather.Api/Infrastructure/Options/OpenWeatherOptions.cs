namespace Aliare.Weather.Api.Infrastructure.Options;

public class OpenWeatherOptions
{
    public const string SectionName = "OpenWeather";
    public required string BaseUrl { get; set; }
    public required string GeoUrl { get; set; }
    public required string ApiKey { get; set; }
}
