using BuildingBlocks.Infrastructure.Configuration;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Utils;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Announcements.Infrastructure.Persistence;
using Scaffold.Weather.Infrastructure.Persistence;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for the full integration-test stack.
/// Applies module schemas before Respawn inspects database tables.
/// </summary>
public sealed class ScaffoldEnvironment : IntegrationTestEnvironment<Program>
{
    public override void LoadEnvironment() => EnvLoader.LoadIntegration(AppContext.BaseDirectory);

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<BuildingBlocks.Core.Abstractions.ICurrentUser, IntegrationCurrentUserHandler>();
    }

    protected override async ValueTask InitializeDatabaseAsync()
    {
        await IntegrationTestDatabaseUtils.MigrateDatabaseAsync<AnnouncementsDbContext>(ConnectionString);
        await IntegrationTestDatabaseUtils.MigrateDatabaseAsync<WeatherDbContext>(ConnectionString);
    }
}