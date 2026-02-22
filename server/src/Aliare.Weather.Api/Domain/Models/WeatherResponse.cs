namespace Aliare.Weather.Api.Domain.Models;

public record WeatherResponse(
    string CityName,
    double Temperature,
    double Latitude,
    double Longitude
);
