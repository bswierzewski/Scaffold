using BuildingBlocks.Tests.Integration.Extensions;
using BuildingBlocks.Tests.Integration.Fixtures;
using BuildingBlocks.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog.Domain.Aggregates;
using Scaffold.Modules.Catalog.Features.GetCatalogItems;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Tests.Integration.Shared;
using System.Net;

namespace Scaffold.Tests.Integration.Features;

[Collection(ScaffoldCollection.Name)]
public sealed class CatalogOneTests(DatabaseFixture databaseFixture)
    : CatalogTestsBase(databaseFixture)
{
    protected override async Task OnInitializeAsync(IServiceProvider services)
    {
        await base.OnInitializeAsync(services);

        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Catalog One Product", "catalog-one-01", 20.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task Get_catalog_items_returns_shared_and_class_product()
    {
        var result = await Host.Scenario(api =>
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
            item => Assert.Equal("Catalog One Product", item.Name));
    }

    [Fact]
    public async Task Get_catalog_items_returns_shared_class_and_test_product()
    {
        using var scope = Host.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Catalog One Test Product", "catalog-one-test-01", 30.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        var result = await Host.Scenario(api =>
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
            item => Assert.Equal("Catalog One Product", item.Name),
            item => Assert.Equal("Catalog One Test Product", item.Name));
    }

    [Fact]
    public async Task Get_catalog_items_requires_explicit_authorization_per_request()
    {
        await Host.Scenario(api =>
        {
            api.RemoveRequestHeader("Authorization");
            api.Get.Url("/api/catalog/items");
            api.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
        });

        await Host.Scenario(api =>
        {
            api.As(new TestCurrentUser(roles: []));
            api.Get.Url("/api/catalog/items");
            api.StatusCodeShouldBe(HttpStatusCode.Forbidden);
        });

        await Host.Scenario(api =>
        {
            api.As(CatalogReaderUser);
            api.Get.Url("/api/catalog/items");
            api.StatusCodeShouldBe(HttpStatusCode.OK);
        });
    }
}