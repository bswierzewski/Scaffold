using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Hosting.Enums;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Modules.Notifications;
using Scaffold.Modules.Notifications.Infrastructure.Persistence;

namespace Scaffold.Api;

/// <summary>
/// Provides bootstrap helpers for registering application modules and running their startup lifecycle.
/// </summary>
public static class ModuleExtensions
{
    /// <summary>
    /// Configures Wolverine for OpenAPI generation without database-backed messaging.
    /// </summary>
    public static void ConfigureOpenApiWolverine(
        this WebApplicationBuilder builder,
        IModule[] modules)
    {
        builder.AddWolverine(modules);
    }

    /// <summary>
    /// Configures Wolverine with the shared PostgreSQL data source for runtime execution.
    /// </summary>
    public static void ConfigureWolverine(
        this WebApplicationBuilder builder,
        IModule[] modules)
    {
        var dataSource = builder.Services.AddPostgresDataSource(builder.Configuration, "Default");
        builder.AddWolverine(modules, dataSource);
    }

    /// <summary>
    /// Creates the application module list, registers each module in DI, and lets modules add their own services.
    /// </summary>
    public static IServiceCollection ConfigureModules(
        this IServiceCollection services,
        IConfiguration configuration,
        out IModule[] modules)
    {
        modules =
        [
            new CatalogModule(),
            new NotificationsModule()
        ];

        foreach (var module in modules)
        {
            services.AddSingleton(typeof(IModule), module);
            module.AddServices(services, configuration);
        }

        return services;
    }

    /// <summary>
    /// Runs module initialization only for executable application modes that require full startup behavior.
    /// </summary>
    public static async Task InitializeModulesAsync(
        this WebApplication app,
        IModule[] modules,
        ApplicationExecutionMode mode)
    {
        if (mode == ApplicationExecutionMode.OpenApi)
            return;

        foreach (var module in modules)
            await module.InitializeAsync(app.Services);
    }

    /// <summary>
    /// Applies owned module migrations explicitly during runtime startup.
    /// </summary>
    public static async Task ApplyMigrations(this WebApplication app)
    {
        Func<IServiceProvider, CancellationToken, Task>[] migrations =
        [
            static (services, cancellationToken) => services.MigrateDatabaseAsync<CatalogDbContext>(cancellationToken),
            static (services, cancellationToken) => services.MigrateDatabaseAsync<NotificationsDbContext>(cancellationToken)
        ];

        foreach (var migration in migrations)
            await migration(app.Services, CancellationToken.None);
    }
}