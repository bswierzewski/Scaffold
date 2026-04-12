using BuildingBlocks.Core.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Scaffold.Weather;

public sealed class WeatherModule : IModule
{
  public static string Name => "Weather";

  public void AddServices(IServiceCollection services, IConfiguration configuration)
  {
  }

  public Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
  {
    return Task.CompletedTask;
  }
}
