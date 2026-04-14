using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Infrastructure.Persistence.DesignTime;
using Microsoft.EntityFrameworkCore;
using Scaffold.Announcements.Domain;

namespace Scaffold.Announcements.Infrastructure.Persistence;

public sealed class AnnouncementsDbContextFactory : DesignTimeDbContextFactoryBase<AnnouncementsDbContext>;

public sealed class AnnouncementsDbContext(DbContextOptions<AnnouncementsDbContext> options) : DbContext(options)
{
  public DbSet<Announcement> Announcements => Set<Announcement>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.HasDefaultSchema(typeof(AnnouncementsDbContext).ToDbContextSchemaName());
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(AnnouncementsDbContext).Assembly);

    base.OnModelCreating(modelBuilder);
  }
}