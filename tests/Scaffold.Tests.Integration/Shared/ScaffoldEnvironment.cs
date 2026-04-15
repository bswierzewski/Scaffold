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
    protected override void LoadEnvironment() => EnvLoader.LoadIntegration(AppContext.BaseDirectory);

    protected override void ConfigureEnvironmentServices(IServiceCollection services)
    {
        services.AddScoped<BuildingBlocks.Core.Abstractions.ICurrentUser, IntegrationCurrentUserHandler>();
    }

    protected override async ValueTask InitializeEnvironmentAsync()
    {
        await IntegrationTestDatabaseUtils.MigrateDatabaseAsync<AnnouncementsDbContext>(ConnectionString);
        await IntegrationTestDatabaseUtils.MigrateDatabaseAsync<WeatherDbContext>(ConnectionString);
    }
}