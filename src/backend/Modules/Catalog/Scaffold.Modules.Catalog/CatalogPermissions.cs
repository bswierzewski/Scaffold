using BuildingBlocks.Core.Primitives;

namespace Scaffold.Modules.Catalog;

public static class CatalogPermissions
{
    public static readonly Permission ReadItems = new("catalog.items.read", "Read catalog items");
    public static readonly Permission ManageItems = new("catalog.items.write", "Manage catalog items");
}