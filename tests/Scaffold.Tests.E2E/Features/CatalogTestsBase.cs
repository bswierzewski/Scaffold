using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Tests.E2E.Extensions;
using BuildingBlocks.Tests.Models;
using Scaffold.AppHost;
using Scaffold.Modules.Catalog.Domain.Aggregates;
using Scaffold.Modules.Catalog;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Tests.E2E.Shared;

namespace Scaffold.Tests.E2E.Features;

[Collection(ScaffoldCollection.Name)]
public abstract class CatalogTestsBase(ScaffoldEnvironment environment) : IAsyncLifetime
{
    protected ScaffoldEnvironment Environment { get; } = environment;
    protected static ICurrentUser CatalogReaderUser { get; } = new TestCurrentUser(
        roles: ["Catalog.Viewer"],
        permissions: [CatalogPermissions.ReadItemsCode]);

    public virtual async ValueTask InitializeAsync()
    {
        await using var dbContext = await Environment.App.CreateDbContextAsync<CatalogDbContext>(ResourceNames.Database);

        var catalogItem = CatalogItem.Create("Base Catalog Product", "base-01", 10.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}