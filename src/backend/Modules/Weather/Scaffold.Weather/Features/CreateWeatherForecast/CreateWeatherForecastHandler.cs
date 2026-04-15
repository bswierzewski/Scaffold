using BuildingBlocks.Core.Attributes;
using Scaffold.Weather.Domain;
using Scaffold.Weather.Features.GetWeatherForecast;
using Scaffold.Weather.Infrastructure.Persistence;
using Wolverine.Http;

namespace Scaffold.Weather.Features.CreateWeatherForecast;

public record CreateWeatherForecastRequest;

public static class CreateWeatherForecastHandler
{
  [Authorize(Roles = "admin")]
  [WolverinePost("/api/weatherforecast")]
  public static async Task<GetWeatherForecastResponse> Handle(
      CreateWeatherForecastRequest request,
      WeatherDbContext dbContext,
      CancellationToken cancellationToken)
  {
    var weatherForecast = WeatherForecast.Create();

    dbContext.WeatherForecasts.Add(weatherForecast);
    await dbContext.SaveChangesAsync(cancellationToken);

    return new GetWeatherForecastResponse(
        weatherForecast.Id,
        weatherForecast.Date,
        weatherForecast.TemperatureC,
        weatherForecast.Summary);
  }
}