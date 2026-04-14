using BuildingBlocks.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Scaffold.Announcements.Infrastructure.Persistence;

public sealed class AnnouncementsDbContextFactory : IDesignTimeDbContextFactory<AnnouncementsDbContext>
{
  public AnnouncementsDbContext CreateDbContext(string[] args)
  {
    var optionsBuilder = new DbContextOptionsBuilder<AnnouncementsDbContext>();

    optionsBuilder.UseNpgsql(
        connectionString: "Host=_design-time_;Database=_design-time_",
      npgsqlOptionsAction: o => o.MigrationsHistoryTable("__EFMigrationsHistory", typeof(AnnouncementsDbContext).ToDbContextSchemaName())
    );

    return new AnnouncementsDbContext(optionsBuilder.Options);
  }
}