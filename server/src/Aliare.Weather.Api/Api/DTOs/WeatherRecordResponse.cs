namespace Aliare.Weather.Api.Api.DTOs;

public record WeatherRecordResponse(
    int Id,
    string City,
    string? State,
    string? Country,
    double Temperature,
    double Latitude,
    double Longitude,
    DateTime RecordedAt
);
