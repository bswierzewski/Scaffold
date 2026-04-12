using BuildingBlocks.Core.Primitives;

namespace Scaffold.Weather.Domain;

public sealed class WeatherForecast : AuditableEntity<Guid>
{
  private static readonly string[] Summaries = [
    "Freezing", "Bracing", "Chilly",
    "Cool", "Mild", "Warm", "Balmy",
    "Hot", "Sweltering", "Scorching"
  ];

  private WeatherForecast() { }

  public DateOnly Date { get; private set; }

  public int TemperatureC { get; private set; }

  public string Summary { get; private set; } = string.Empty;

  public static WeatherForecast Create()
  {
    return new WeatherForecast
    {
      Id = Guid.NewGuid(),
      Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(Random.Shared.Next(1, 15))),
      TemperatureC = Random.Shared.Next(-20, 55),
      Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    };
  }
}