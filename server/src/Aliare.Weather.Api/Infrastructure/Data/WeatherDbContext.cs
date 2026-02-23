using Aliare.Weather.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aliare.Weather.Api.Infrastructure.Data;

public class WeatherDbContext(DbContextOptions<WeatherDbContext> options) : DbContext(options)
{
    public DbSet<WeatherRecord> WeatherRecords => Set<WeatherRecord>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatherDbContext).Assembly);
    }
}
