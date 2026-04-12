namespace Scaffold.Weather.Features.GetWeatherForecast;

public record GetWeatherForecastResponse(
  Guid Id,
  DateOnly Date,
  int TemperatureC,
  string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}