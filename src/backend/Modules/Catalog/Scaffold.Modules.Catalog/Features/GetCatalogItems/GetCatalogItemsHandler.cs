using BuildingBlocks.Core.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Wolverine.Http;

namespace Scaffold.Modules.Catalog.Features.GetCatalogItems;

public sealed class GetCatalogItemsHandler
{
    [WolverineGet("/api/catalog/items")]
    [Tags("Catalog")]
    [EndpointName("GetCatalogItems")]
    [EndpointSummary("Get all catalog items")]
    [Authorize(Permissions = [CatalogPermissions.ReadItemsCode])]
    public static async Task<List<GetCatalogItemsResponse>> Handle(
        CatalogDbContext dbContext,
        CancellationToken ct)
    {
        return await dbContext.CatalogItems
            .AsNoTracking()
            .OrderBy(item => item.Name)
            .Select(item => new GetCatalogItemsResponse(
                item.Id,
                item.Name,
                item.Sku,
                item.Price))
            .ToListAsync(ct);
    }
}