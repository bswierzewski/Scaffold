using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Scaffold.Modules.Catalog.Domain.Aggregates;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Modules.Catalog.Infrastructure.Services;
using Wolverine.Http;

namespace Scaffold.Modules.Catalog.Features.CreateCatalogItem;

public sealed class CreateCatalogItemHandler
{
    [WolverinePost("/catalog/items")]
    [Tags("Catalog")]
    [EndpointName("CreateCatalogItem")]
    [EndpointSummary("Create a catalog item")]
    public static async Task<CreateCatalogItemResponse> Handle(
        CreateCatalogItemCommand command,
        CatalogDbContext dbContext,
        IPrinter printer,
        CancellationToken ct)
    {
        var item = CatalogItem.Create(command.Name, command.Sku, command.Price);

        await dbContext.CatalogItems.AddAsync(item, ct);

        printer.Print($"[Catalog] Created item '{item.Name}' with SKU '{item.Sku}'.");

        return new CreateCatalogItemResponse(item.Id);
    }
}