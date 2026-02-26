using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Aliare.Weather.Api.Infrastructure.Data;

public class WeatherDbContextFactory : IDesignTimeDbContextFactory<WeatherDbContext>
{
    public WeatherDbContext CreateDbContext(string[] args)
    {
        DbContextOptions<WeatherDbContext> options = new DbContextOptionsBuilder<WeatherDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=aliare_weather;Username=postgres;Password=postgres")
            .Options;

        return new WeatherDbContext(options);
    }
}
