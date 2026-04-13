using BuildingBlocks.Tests.Integration;
using Microsoft.EntityFrameworkCore;
using Scaffold.Weather;
using Scaffold.Weather.Infrastructure.Persistence;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for all Weather integration tests.
/// Applies the module schema before Respawn inspects database tables.
/// </summary>
public sealed class WeatherEnvironment : IntegrationTestEnvironment<Program>
{
  /// <summary>
  /// Applies pending EF Core migrations so the schema exists before
  /// Respawn inspects the database tables.
  /// The MigrationsHistoryTable schema must match what AddPostgres registers
  /// so that ModuleInitializerService (running later inside the AlbaHost) sees
  /// the migration as already applied and skips it.
  /// </summary>
  protected override async ValueTask InitializeDatabaseAsync()
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