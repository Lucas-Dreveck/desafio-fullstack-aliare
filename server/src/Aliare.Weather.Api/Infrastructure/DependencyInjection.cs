using Aliare.Weather.Api.Domain.Interfaces;
using Aliare.Weather.Api.Infrastructure.Data;
using Aliare.Weather.Api.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Aliare.Weather.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WeatherDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IWeatherRepository, WeatherRepository>();

        return services;
    }
}
