using BuildingBlocks.Tests.Integration;
using Microsoft.EntityFrameworkCore;
using Scaffold.Announcements;
using Scaffold.Announcements.Infrastructure.Persistence;
using Scaffold.Weather;
using Scaffold.Weather.Infrastructure.Persistence;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for the full integration-test stack.
/// Applies module schemas before Respawn inspects database tables.
/// </summary>
public sealed class ScaffoldEnvironment : IntegrationTestEnvironment<Program>
{
  protected override async ValueTask InitializeDatabaseAsync()
  {
    await MigrateAnnouncementsAsync();
    await MigrateWeatherAsync();
  }

  private async Task MigrateAnnouncementsAsync()
  {
    var schema = AnnouncementsModule.Name.ToLowerInvariant();

    var options = new DbContextOptionsBuilder<AnnouncementsDbContext>()
        .UseNpgsql(ConnectionString, np =>
            np.MigrationsHistoryTable("__EFMigrationsHistory", schema))
        .Options;

    await using var dbContext = new AnnouncementsDbContext(options);
    await dbContext.Database.MigrateAsync();
  }

  private async Task MigrateWeatherAsync()
  {
    var schema = WeatherModule.Name.ToLowerInvariant();

    var options = new DbContextOptionsBuilder<WeatherDbContext>()
        .UseNpgsql(ConnectionString, np =>
            np.MigrationsHistoryTable("__EFMigrationsHistory", schema))
        .Options;

    await using var dbContext = new WeatherDbContext(options);
    await dbContext.Database.MigrateAsync();
  }
}