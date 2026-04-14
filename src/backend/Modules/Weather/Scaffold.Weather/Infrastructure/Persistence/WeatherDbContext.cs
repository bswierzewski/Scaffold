using BuildingBlocks.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Scaffold.Weather.Domain;

namespace Scaffold.Weather.Infrastructure.Persistence;

public sealed class WeatherDbContext(DbContextOptions<WeatherDbContext> options) : DbContext(options)
{
  public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(typeof(WeatherDbContext).ToDbContextSchemaName());
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatherDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
  }
}