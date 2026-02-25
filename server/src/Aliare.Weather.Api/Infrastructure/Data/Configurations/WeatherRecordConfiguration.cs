using Aliare.Weather.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aliare.Weather.Api.Infrastructure.Data.Configurations;

public class WeatherRecordConfiguration : IEntityTypeConfiguration<WeatherRecord>
{
    public void Configure(EntityTypeBuilder<WeatherRecord> builder)
    {
        builder.ToTable("weather_records");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.City)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.State)
            .HasMaxLength(200);

        builder.Property(e => e.Country)
            .HasMaxLength(10);

        builder.Property(e => e.Temperature)
            .IsRequired();

        builder.Property(e => e.Latitude)
            .IsRequired();

        builder.Property(e => e.Longitude)
            .IsRequired();

        builder.Property(e => e.RecordedAt)
            .IsRequired();

        builder.HasIndex(e => e.City);
        builder.HasIndex(e => new { e.Latitude, e.Longitude });
    }
}
