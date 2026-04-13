using BuildingBlocks.Tests.Integration;
using Microsoft.EntityFrameworkCore;
using Scaffold.Weather;
using Scaffold.Weather.Infrastructure.Persistence;
using Xunit;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// xUnit collection fixture shared by all Weather integration tests.
/// Manages the PostgreSQL container lifecycle and initialises the schema
/// by applying EF Core migrations before Respawn inspects the tables.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class WeatherCollection : IntegrationTestCollection<Program>, ICollectionFixture<WeatherCollection>
{
    public const string Name = "Weather";

    /// <summary>
    /// Applies pending EF Core migrations so the schema exists before
    /// Respawner introspects the database tables.
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
