using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using BuildingBlocks.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog;
using Scaffold.Modules.Catalog.Domain.Aggregates;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Tests.Integration.Shared;

namespace Scaffold.Tests.Integration.Features;

[Collection(ScaffoldCollection.Name)]
public abstract class CatalogTestsBase(DatabaseFixture databaseFixture) : IntegrationTestBase<Program>(databaseFixture)
{
    protected static ICurrentUser CatalogReaderUser { get; } = new TestCurrentUser(
        roles: ["Catalog.Viewer"],
        permissions: [CatalogPermissions.ReadItemsCode]);

    protected override async Task OnInitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Base Catalog Product", "base-01", 10.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}