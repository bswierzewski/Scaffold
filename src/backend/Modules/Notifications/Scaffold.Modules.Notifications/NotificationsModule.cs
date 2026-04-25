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
    public string Name => "Notifications";

    public IReadOnlyCollection<Permission> Permissions =>
    [
        new("notifications.messages.read", "Read notification messages"),
        new("notifications.messages.write", "Manage notification messages")
    ];

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