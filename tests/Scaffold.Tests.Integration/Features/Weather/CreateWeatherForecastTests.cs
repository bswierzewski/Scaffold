using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Extensions;
using Scaffold.Tests.Integration.Shared;
using Scaffold.Weather.Features.GetWeatherForecast;

namespace Scaffold.Tests.Integration.Features.Weather;

[Collection(ScaffoldCollection.Name)]
public sealed class CreateWeatherForecastTests(ScaffoldEnvironment environment, ITestOutputHelper output)
    : IntegrationTestBase<Program>(environment)
{
    [Fact]
    public async Task should_create_forecast_and_return_valid_response()
    {
        var result = await AlbaHost.Scenario(s =>
        {
            s.Post.Json(new { }).ToUrl("/api/weatherforecast");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output, "Created forecast:");

        var forecast = await result.ReadAsJsonAsync<GetWeatherForecastResponse>();

        Assert.NotNull(forecast);
        Assert.NotEqual(Guid.Empty, forecast.Id);
        Assert.NotEqual(default, forecast.Date);
        Assert.InRange(forecast.TemperatureC, -20, 55);
        Assert.NotNull(forecast.Summary);
    }

    [Fact]
    public async Task created_forecast_should_appear_in_get_results()
    {
        var createResult = await AlbaHost.Scenario(s =>
        {
            s.Post.Json(new { }).ToUrl("/api/weatherforecast");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var created = await createResult.ReadAsJsonAsync<GetWeatherForecastResponse>();
        Assert.NotNull(created);

        var listResult = await AlbaHost.Scenario(s =>
        {
            s.Get.Url("/api/weatherforecast");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output, "Forecast list after create:");

        var forecasts = await listResult.ReadAsJsonAsync<GetWeatherForecastResponse[]>();

        Assert.NotNull(forecasts);
        Assert.Single(forecasts);
        Assert.Equal(created.Id, forecasts[0].Id);
        Assert.Equal(created.Date, forecasts[0].Date);
        Assert.Equal(created.TemperatureC, forecasts[0].TemperatureC);
    }
}