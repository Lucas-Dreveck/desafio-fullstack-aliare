using Aliare.Weather.Api.Domain.Entities;

namespace Aliare.Weather.Api.Tests.Domain.Weather;

public class WeatherRecordTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateRecord()
    {
        Assert.NotNull(new WeatherRecord("Test City", 25.0, -00.0, -00.0, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_WithEmptyCity_ShouldThrowException()
    {
        string city = string.Empty;

        Assert.Throws<ArgumentException>(() => new WeatherRecord(city, 25.0, -00.0, -00.0, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_WithNullCity_ShouldThrowException()
    {
        string? city = null;

        Assert.Throws<ArgumentNullException>(() => new WeatherRecord(city!, 25.0, -00.0, -00.0, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_WithInvalidLatitude_ShouldThrowException()
    {
        double latitude = 100.0;

        Assert.Throws<ArgumentOutOfRangeException>(() => new WeatherRecord("Test City", 25.0, latitude, -00.0, DateTime.UtcNow));
    }

    [Fact]
    public void Constructor_WithInvalidLongitude_ShouldThrowException()
    {
        double longitude = 200.0;

        Assert.Throws<ArgumentOutOfRangeException>(() => new WeatherRecord("Test City", 25.0, -00.0, longitude, DateTime.UtcNow));
    }
}
