namespace Aliare.Weather.Api.Api.DTOs;

public record WeatherRecordResponse(
    int Id,
    string CityName,
    double Temperature,
    double Latitude,
    double Longitude,
    DateTime RecordedAt
);
