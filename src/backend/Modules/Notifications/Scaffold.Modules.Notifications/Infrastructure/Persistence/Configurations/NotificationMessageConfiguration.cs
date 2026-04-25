using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scaffold.Modules.Notifications.Domain.Aggregates;

namespace Scaffold.Modules.Notifications.Infrastructure.Persistence.Configurations;

internal sealed class NotificationMessageConfiguration : IEntityTypeConfiguration<NotificationMessage>
{
    public void Configure(EntityTypeBuilder<NotificationMessage> builder)
    {
        builder.Property(x => x.Title)
            .IsRequired();

        builder.Property(x => x.Body)
            .IsRequired();
    }
}