using BuildingBlocks.Core.Abstractions;
using Scaffold.Announcements;
using Scaffold.Weather;

namespace Scaffold.Bootstrapping;

/// <summary>
/// Provides the canonical list of application modules shared by bootstrappers.
/// </summary>
public static class ScaffoldModules
{
  public static IModule[] Create() =>
  [
      new AnnouncementsModule(),
      new WeatherModule()
  ];
}