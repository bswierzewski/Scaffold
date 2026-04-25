using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scaffold.Modules.Notifications.Domain.Aggregates;

namespace Scaffold.Modules.Notifications.Infrastructure.Persistence;

public sealed class Factory : ModuleDbContextDesignTimeFactory<NotificationsDbContext> { }

public sealed class NotificationsDbContext(DbContextOptions<NotificationsDbContext> options)
    : ModuleDbContext<NotificationsDbContext>(options, SchemaName)
{
    public const string SchemaName = "notifications";

    public DbSet<NotificationMessage> NotificationMessages => Set<NotificationMessage>();
}