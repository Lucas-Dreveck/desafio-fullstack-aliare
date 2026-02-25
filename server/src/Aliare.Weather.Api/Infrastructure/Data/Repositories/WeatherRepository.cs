using Aliare.Weather.Api.Domain.Entities;
using Aliare.Weather.Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Aliare.Weather.Api.Infrastructure.Data.Repositories;

public class WeatherRepository(WeatherDbContext context) : IWeatherRepository
{
    public async Task AddAsync(WeatherRecord record)
    {
        await context.WeatherRecords.AddAsync(record);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<WeatherRecord>> GetByCityAsync(string cityName, int days = 30)
    {
        DateTime since = DateTime.UtcNow.AddDays(-days);

        return await context.WeatherRecords
            .Where(r => r.CityName == cityName && r.RecordedAt >= since)
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WeatherRecord>> GetByCoordinatesAsync(double latitude, double longitude, int days = 30)
    {
        DateTime since = DateTime.UtcNow.AddDays(-days);
        const double tolerance = 0.0001;

        return await context.WeatherRecords
            .Where(r => Math.Abs(r.Latitude - latitude) < tolerance
                     && Math.Abs(r.Longitude - longitude) < tolerance
                     && r.RecordedAt >= since)
            .OrderByDescending(r => r.RecordedAt)
            .ToListAsync();
    }
}
