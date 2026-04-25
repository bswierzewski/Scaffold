namespace Scaffold.Modules.Catalog.Features.CreateCatalogItem;

public sealed record CreateCatalogItemCommand(
    string Name,
    string Sku,
    decimal Price);

public sealed record CreateCatalogItemResponse(Guid Id);