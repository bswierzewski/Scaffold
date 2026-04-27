using BuildingBlocks.Core.Primitives;
using BuildingBlocks.Infrastructure.Identity;
using Scaffold.Modules.Catalog;
using Scaffold.Modules.Notifications;

namespace Scaffold.Api.Authorization;

/// <summary>
/// Defines application roles and the permissions granted by each role.
/// </summary>
public sealed class ScaffoldRolePermissionMap : RolePermissionMap
{
    protected override IEnumerable<Role> GetRoles()
    {
        yield return new Role(
            "admin",
            [
                CatalogPermissions.ReadItems,
                CatalogPermissions.ManageItems,
                NotificationsPermissions.ReadMessages,
                NotificationsPermissions.ManageMessages
            ]);

        yield return new Role(
            "reader",
            [
                CatalogPermissions.ReadItems,
                NotificationsPermissions.ReadMessages
            ]);
    }
}