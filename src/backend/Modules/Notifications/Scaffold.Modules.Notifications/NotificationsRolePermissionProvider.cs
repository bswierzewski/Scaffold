using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Core.Primitives;

namespace Scaffold.Modules.Notifications;

/// <summary>
/// Provides role-to-permission mappings for the Notifications module.
/// </summary>
public sealed class NotificationsRolePermissionProvider : IRolePermissionProvider
{
    public IEnumerable<Role> GetRolePermissions()
    {
        yield return new Role("Notifications.Viewer", [NotificationsPermissions.ReadMessages]);
        yield return new Role("Notifications.Manager", [NotificationsPermissions.ReadMessages, NotificationsPermissions.ManageMessages]);
        yield return new Role("Admin", [NotificationsPermissions.ReadMessages, NotificationsPermissions.ManageMessages]);
    }
}
