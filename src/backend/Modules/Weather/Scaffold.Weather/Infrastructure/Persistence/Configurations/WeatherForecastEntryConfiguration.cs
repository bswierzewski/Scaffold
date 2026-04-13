using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scaffold.Weather.Domain;

namespace Scaffold.Weather.Infrastructure.Persistence.Configurations;

public sealed class WeatherForecastEntryConfiguration : IEntityTypeConfiguration<WeatherForecast>
{
    public void Configure(EntityTypeBuilder<WeatherForecast> builder)
    {
        const int userIdMaxLength = 255;

        builder.ToTable("WeatherForecasts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.TemperatureC)
            .IsRequired();

        builder.Property(x => x.Summary)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .HasMaxLength(userIdMaxLength)
            .IsRequired();

        builder.Property(x => x.ModifiedBy)
            .HasMaxLength(userIdMaxLength)
            .IsRequired();
    }
}