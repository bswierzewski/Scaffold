using Wolverine.Http;

namespace Scaffold.Weather.Features.GetWeatherForecast;

public static class GetWeatherForecastHandler
{
  [WolverineGet("/api/weatherforecast")]
  public static GetWeatherForecastResponse[] Handle()
  {
    string[] summaries = [
      "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    return Enumerable.Range(1, 5)
        .Select(index => new GetWeatherForecastResponse(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]))
        .ToArray();
  }
}