using BuildingBlocks.Core.Primitives;

namespace Scaffold.Modules.Catalog.Domain.Aggregates;

public sealed class CatalogItem : AuditableEntity<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public decimal Price { get; private set; }

    private CatalogItem() { }

    private CatalogItem(Guid id, string name, string sku, decimal price)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);

        Id = id;
        Name = name.Trim();
        Sku = sku.Trim().ToUpperInvariant();
        Price = price;
    }

    public static CatalogItem Create(string name, string sku, decimal price)
        => new(Guid.CreateVersion7(), name, sku, price);
}