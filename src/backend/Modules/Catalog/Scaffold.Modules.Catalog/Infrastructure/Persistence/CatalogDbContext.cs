using BuildingBlocks.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Scaffold.Modules.Catalog.Domain.Aggregates;

namespace Scaffold.Modules.Catalog.Infrastructure.Persistence;

public sealed class Factory : ModuleDbContextDesignTimeFactory<CatalogDbContext> { }

public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options)
    : ModuleDbContext<CatalogDbContext>(options, SchemaName)
{
    public const string SchemaName = "catalog";

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
}