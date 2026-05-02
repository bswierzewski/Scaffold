using BuildingBlocks.Tests.Integration.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog.Domain.Aggregates;
using Scaffold.Modules.Catalog.Features.GetCatalogItems;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Tests.Integration.Shared;
using System.Net;

namespace Scaffold.Tests.Integration.Features;

[Collection(ScaffoldCollection.Name)]
public sealed class CatalogTwoTests(ScaffoldEnvironment environment)
    : CatalogTestsBase(environment)
{
    public override async ValueTask InitializeAsync()
    {
        await base.InitializeAsync();

        using var scope = Environment.Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Catalog Two Product", "catalog-two-01", 25.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Get_catalog_items_returns_shared_and_class_product()
    {
        var result = await Environment.Host.Scenario(api =>
        {
            api.As(CatalogReaderUser);
            api.Get.Url("/api/catalog/items");
            api.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<List<GetCatalogItemsResponse>>();

        Assert.NotNull(items);
        Assert.Collection(
            items,
            item => Assert.Equal("Base Catalog Product", item.Name),
            item => Assert.Equal("Catalog Two Product", item.Name));
    }

    [Fact]
    public async Task Get_catalog_items_returns_shared_class_and_test_product()
    {
        using var scope = Environment.Host.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Catalog Two Test Product", "catalog-two-test-01", 35.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await Environment.Host.Scenario(api =>
        {
            api.As(CatalogReaderUser);
            api.Get.Url("/api/catalog/items");
            api.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var items = await result.ReadAsJsonAsync<List<GetCatalogItemsResponse>>();

        Assert.NotNull(items);
        Assert.Collection(
            items,
            item => Assert.Equal("Base Catalog Product", item.Name),
            item => Assert.Equal("Catalog Two Product", item.Name),
            item => Assert.Equal("Catalog Two Test Product", item.Name));
    }
}