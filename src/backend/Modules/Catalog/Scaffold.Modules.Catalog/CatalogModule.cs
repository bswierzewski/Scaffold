using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Modules.Catalog.Infrastructure.Services;

namespace Scaffold.Modules.Catalog;

public sealed class CatalogModule : IModule
{
    public string Name => "Catalog";

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPrinter, ConsolePrinter>();
        services.AddPostgres<CatalogDbContext>(CatalogDbContext.SchemaName);
    }

    public async Task InitializeMigrationsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await services.MigrateDatabaseAsync<CatalogDbContext>(cancellationToken);
    }
}