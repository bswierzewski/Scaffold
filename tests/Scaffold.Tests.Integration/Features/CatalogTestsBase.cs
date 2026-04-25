using Microsoft.Extensions.DependencyInjection;
using Scaffold.Modules.Catalog.Domain.Aggregates;
using Scaffold.Modules.Catalog.Infrastructure.Persistence;
using Scaffold.Tests.Integration.Shared;

namespace Scaffold.Tests.Integration.Features;

[Collection(ScaffoldCollection.Name)]
public abstract class CatalogTestsBase(ScaffoldEnvironment environment) : IAsyncLifetime
{
    protected ScaffoldEnvironment Environment { get; } = environment;

    public virtual async ValueTask InitializeAsync()
    {
        await Environment.ResetDatabaseAsync();

        using var scope = Environment.Host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

        var catalogItem = CatalogItem.Create("Base Catalog Product", "base-01", 10.00m);
        await dbContext.CatalogItems.AddAsync(catalogItem, TestContext.Current.CancellationToken);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    public ValueTask DisposeAsync() => new(Environment.ResetDatabaseAsync());
}