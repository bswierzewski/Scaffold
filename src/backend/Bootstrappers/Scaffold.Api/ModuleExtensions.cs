using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Hosting.Enums;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog;
using Scaffold.Modules.Notifications;

namespace Scaffold.Api;

/// <summary>
/// Provides bootstrap helpers for registering application modules and running their startup lifecycle.
/// </summary>
public static class ModuleExtensions
{
    /// <summary>
    /// Configures Wolverine for the current execution mode, skipping database-backed messaging for OpenAPI generation.
    /// </summary>
    public static void ConfigureWolverine(
        this WebApplicationBuilder builder,
        ApplicationExecutionMode mode,
        IModule[] modules)
    {
        if (mode == ApplicationExecutionMode.OpenApi)
        {
            builder.AddWolverine(modules);
            return;
        }

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
    /// Applies module migrations only when the host is running outside the metadata-only OpenAPI mode.
    /// </summary>
    public static async Task RunMigrationsAsync(
        this WebApplication app,
        IModule[] modules,
        ApplicationExecutionMode mode)
    {
        if (mode == ApplicationExecutionMode.OpenApi)
            return;

        foreach (var module in modules)
            await module.InitializeMigrationsAsync(app.Services);
    }
}