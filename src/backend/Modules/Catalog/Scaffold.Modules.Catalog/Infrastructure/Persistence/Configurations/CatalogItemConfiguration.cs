using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scaffold.Modules.Catalog.Domain.Aggregates;

namespace Scaffold.Modules.Catalog.Infrastructure.Persistence.Configurations;

internal sealed class CatalogItemConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired();

        builder.Property(x => x.Sku)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.HasIndex(x => x.Sku)
            .IsUnique();
    }
}