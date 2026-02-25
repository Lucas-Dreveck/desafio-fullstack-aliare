namespace Aliare.Weather.Api.Domain.Entities;

public class WeatherRecord
{
    public int Id { get; init; }
    public string City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public double Temperature { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public DateTime RecordedAt { get; private set; }

    public WeatherRecord(string city, double temperature, double latitude, double longitude, DateTime recordedAt, string? state = null, string? country = null)
    {
        ValidateCity(city);
        ValidateLatitude(latitude);
        ValidateLongitude(longitude);

        City = city;
        State = state;
        Country = country;
        Temperature = temperature;
        Latitude = latitude;
        Longitude = longitude;
        RecordedAt = recordedAt;
    }

    public static void ValidateCity(string city)
    {
        ArgumentNullException.ThrowIfNull(city);
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City name cannot be empty.", nameof(city));
    }

    public static void ValidateLatitude(double latitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
    }

    public static void ValidateLongitude(double longitude)
    {
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
    }

}
