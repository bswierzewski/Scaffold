namespace Scaffold.Modules.Catalog.Features.GetCatalogItems;

public sealed record GetCatalogItemsResponse(
    Guid Id,
    string Name,
    string Sku,
    decimal Price);