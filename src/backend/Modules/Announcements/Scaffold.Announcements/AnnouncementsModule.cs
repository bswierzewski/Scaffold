using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Announcements.Infrastructure.Persistence;

namespace Scaffold.Announcements;

public sealed class AnnouncementsModule : IModule
{
  public static string Name => "Announcements";

  public void AddServices(IServiceCollection services, IConfiguration configuration)
  {
    services.AddPostgres<AnnouncementsDbContext>(Name);
  }

  public Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
  {
    return services.MigrateDatabaseAsync<AnnouncementsDbContext>(cancellationToken);
  }
}