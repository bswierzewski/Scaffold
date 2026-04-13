using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Scaffold.Announcements.Domain;

namespace Scaffold.Announcements.Infrastructure.Persistence.Configurations;

public sealed class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
{
  public void Configure(EntityTypeBuilder<Announcement> builder)
  {
    const int userIdMaxLength = 255;

    builder.ToTable("Announcements");

    builder.HasKey(x => x.Id);

    builder.Property(x => x.Id)
        .ValueGeneratedNever();

    builder.Property(x => x.Title)
        .HasMaxLength(200)
        .IsRequired();

    builder.Property(x => x.Content)
        .HasMaxLength(2000)
        .IsRequired();

    builder.Property(x => x.PublishedAt)
        .IsRequired();

    builder.Property(x => x.CreatedAt)
        .IsRequired();

    builder.Property(x => x.CreatedBy)
        .HasMaxLength(userIdMaxLength)
        .IsRequired();

    builder.Property(x => x.ModifiedBy)
        .HasMaxLength(userIdMaxLength)
        .IsRequired();
  }
}