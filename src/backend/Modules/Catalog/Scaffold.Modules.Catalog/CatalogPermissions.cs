using BuildingBlocks.Core.Primitives;

namespace Scaffold.Modules.Catalog;

public static class CatalogPermissions
{
    public const string ReadItemsCode = "catalog.items.read";
    public const string ManageItemsCode = "catalog.items.write";

    public static readonly Permission ReadItems = new(ReadItemsCode, "Read catalog items");
    public static readonly Permission ManageItems = new(ManageItemsCode, "Manage catalog items");
}