using Microsoft.EntityFrameworkCore;
using Scaffold.Weather.Infrastructure.Persistence;
using Wolverine.Http;

namespace Scaffold.Weather.Features.GetWeatherForecast;

public static class GetWeatherForecastHandler
{
  [WolverineGet("/api/weatherforecast")]
  public static async Task<GetWeatherForecastResponse[]> Handle(
      WeatherDbContext dbContext,
      CancellationToken cancellationToken)
  {
    return await dbContext.WeatherForecasts
        .AsNoTracking()
        .OrderBy(x => x.Date)
        .Select(x => new GetWeatherForecastResponse(
            x.Id,
            x.Date,
            x.TemperatureC,
            x.Summary))
        .ToArrayAsync(cancellationToken);
  }
}