using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scaffold.Weather.Infrastructure.Persistence;

namespace Scaffold.Weather;

public sealed class WeatherModule : IModule
{
  public static string Name => "Weather";

  public void AddServices(IServiceCollection services, IConfiguration configuration)
  {
    services.AddPostgres<WeatherDbContext>(Name);
  }

  public async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
  {
    await services.MigrateDatabaseAsync<WeatherDbContext>(cancellationToken);
  }
}
