namespace Aliare.Weather.Api.Domain.Models;

public record WeatherResponse(
    string City,
    string? State,
    string? Country,
    double Temperature,
    double Latitude,
    double Longitude
);
