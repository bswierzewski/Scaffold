using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Core.Primitives;

namespace Scaffold.Modules.Catalog;

/// <summary>
/// Provides role-to-permission mappings for the Catalog module.
/// </summary>
public sealed class CatalogRolePermissionProvider : IRolePermissionProvider
{
    public IEnumerable<Role> GetRolePermissions()
    {
        yield return new Role("Catalog.Viewer", [CatalogPermissions.ReadItems]);
        yield return new Role("Catalog.Manager", [CatalogPermissions.ReadItems, CatalogPermissions.ManageItems]);
        yield return new Role("Admin", [CatalogPermissions.ReadItems, CatalogPermissions.ManageItems]);
    }
}
