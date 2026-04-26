using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Core.Primitives;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Notifications.Infrastructure.Persistence;
using Scaffold.Modules.Notifications.Infrastructure.Services;

namespace Scaffold.Modules.Notifications;

public sealed class NotificationsModule : IModule
{
    private static readonly Permission ReadMessages = new("notifications.messages.read", "Read notification messages");
    private static readonly Permission ManageMessages = new("notifications.messages.write", "Manage notification messages");
    private static readonly IReadOnlyCollection<Permission> ModulePermissions = [ReadMessages, ManageMessages];
    private static readonly IReadOnlyCollection<Role> ModuleRoles =
    [
        new Role("notifications.reader", [ReadMessages]),
        new Role("notifications.manager", [ReadMessages, ManageMessages])
    ];

    public string Name => "Notifications";

    public IReadOnlyCollection<Permission> Permissions => ModulePermissions;

    public IReadOnlyCollection<Role> Roles => ModuleRoles;

    public void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPrinter, ConsolePrinter>();
        services.AddPostgres<NotificationsDbContext>(NotificationsDbContext.SchemaName);
    }

    public async Task InitializeMigrationsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await services.MigrateDatabaseAsync<NotificationsDbContext>(cancellationToken);
    }
}