using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Domain.Models;
using Aliare.Weather.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Aliare.Weather.Api.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly Mock<IWeatherProvider> _mockWeatherProvider = new();
    private readonly string _dbName = "TestDb_" + Guid.NewGuid().ToString();

    public Mock<IWeatherProvider> MockWeatherProvider => _mockWeatherProvider;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ServiceDescriptor? dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<WeatherDbContext>));
            if (dbDescriptor is not null)
                services.Remove(dbDescriptor);

            string dbName = _dbName;
            services.AddDbContext<WeatherDbContext>(options =>
                options.UseInMemoryDatabase(dbName));

            ServiceDescriptor? providerDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IWeatherProvider));
            if (providerDescriptor is not null)
                services.Remove(providerDescriptor);

            _mockWeatherProvider
                .Setup(p => p.GetByCityAsync("Cascavel", null, null))
                .ReturnsAsync(new WeatherResponse("Cascavel", 25.0, -24.95, -53.45));

            _mockWeatherProvider
                .Setup(p => p.GetByCoordinatesAsync(-24.95, -53.45))
                .ReturnsAsync(new WeatherResponse("Cascavel", 25.0, -24.95, -53.45));

            services.AddScoped<IWeatherProvider>(_ => _mockWeatherProvider.Object);
        });
    }
}
