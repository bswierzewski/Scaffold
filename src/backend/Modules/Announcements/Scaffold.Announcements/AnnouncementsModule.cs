using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Announcements.Infrastructure.Persistence;

namespace Scaffold.Announcements;

public sealed class AnnouncementsModule : IModule
{
  public string Name => nameof(AnnouncementsModule);

  public void AddServices(IServiceCollection services, IConfiguration configuration)
  {
    services.AddPostgres<AnnouncementsDbContext>();
  }

  public Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
      => Task.CompletedTask;

  public Task InitializeMigrationsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
      => services.MigrateDatabaseAsync<AnnouncementsDbContext>(cancellationToken);
}