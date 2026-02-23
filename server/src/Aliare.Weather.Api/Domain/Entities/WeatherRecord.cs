namespace Aliare.Weather.Api.Domain.Entities;

public class WeatherRecord
{
    public int Id { get; private set; }
    public string CityName { get; private set; }
    public double Temperature { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public DateTime RecordedAt { get; private set; }

    public WeatherRecord(string cityName, double temperature, double latitude, double longitude, DateTime recordedAt)
    {
        ValidateCityName(cityName);
        ValidateLatitude(latitude);
        ValidateLongitude(longitude);

        CityName = cityName;
        Temperature = temperature;
        Latitude = latitude;
        Longitude = longitude;
        RecordedAt = recordedAt;
    }

    private static void ValidateCityName(string cityName)
    {
        ArgumentNullException.ThrowIfNull(cityName);
        if (string.IsNullOrWhiteSpace(cityName))
            throw new ArgumentException("City name cannot be empty.", nameof(cityName));
    }

    private static void ValidateLatitude(double latitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");
    }

    private static void ValidateLongitude(double longitude)
    {
        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");
    }

}
