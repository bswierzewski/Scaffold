using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Core.Primitives;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Modules.Catalog.Infrastructure.Services;

namespace Scaffold.Modules.Catalog;

public sealed class CatalogModule : IModule
{
    private static readonly Permission ReadItems = new("catalog.items.read", "Read catalog items");
    private static readonly Permission ManageItems = new("catalog.items.write", "Manage catalog items");
    private static readonly IReadOnlyCollection<Permission> ModulePermissions = [ReadItems, ManageItems];
    private static readonly IReadOnlyCollection<Role> ModuleRoles =
    [
        new Role(SystemRoles.Moderator, [ReadItems]),
        new Role(SystemRoles.Admin, [ReadItems, ManageItems]),
        new Role("catalog.reader", [ReadItems]),
        new Role("catalog.manager", [ReadItems, ManageItems])
    ];

    public string Name => "Catalog";

    public IReadOnlyCollection<Permission> Permissions => ModulePermissions;

    public IReadOnlyCollection<Role> Roles => ModuleRoles;

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