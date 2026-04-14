using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Weather.Infrastructure.Persistence;

namespace Scaffold.Weather;

public sealed class WeatherModule : IModule
{
  public string Name => nameof(WeatherModule);

  public void AddServices(IServiceCollection services, IConfiguration configuration)
  {
    services.AddPostgres<WeatherDbContext>();
  }

  public Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
      => Task.CompletedTask;

  public Task InitializeMigrationsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
      => services.MigrateDatabaseAsync<WeatherDbContext>(cancellationToken);
}
