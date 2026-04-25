using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog.Domain.Aggregates;
using Scaffold.Modules.Catalog.Features.GetCatalogItems;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Tests.Integration.Shared;

namespace Scaffold.Tests.Integration.Features;

[Collection(ScaffoldCollection.Name)]
public sealed class CatalogOneTests(ScaffoldEnvironment environment)
    : CatalogTestsBase(environment)
{
    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        using var scope = Environment.Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Catalog One Product", "catalog-one-01", 20.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Get_catalog_items_returns_shared_and_class_product()
    {
        using var client = Environment.Host.Server.CreateClient();
        using var response = await client.GetAsync("/api/catalog/items", TestContext.Current.CancellationToken);
        var items = await response.Content.ReadFromJsonAsync<List<GetCatalogItemsResponse>>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.Collection(
            items,
            item => Assert.Equal("Base Catalog Product", item.Name),
            item => Assert.Equal("Catalog One Product", item.Name));
    }

    [Fact]
    public async Task Get_catalog_items_returns_shared_class_and_test_product()
    {
        using var scope = Environment.Host.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Catalog One Test Product", "catalog-one-test-01", 30.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        using var client = Environment.Host.Server.CreateClient();
        using var response = await client.GetAsync("/api/catalog/items", TestContext.Current.CancellationToken);
        var items = await response.Content.ReadFromJsonAsync<List<GetCatalogItemsResponse>>(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
        Assert.Collection(
            items,
            item => Assert.Equal("Base Catalog Product", item.Name),
            item => Assert.Equal("Catalog One Product", item.Name),
            item => Assert.Equal("Catalog One Test Product", item.Name));
    }
}