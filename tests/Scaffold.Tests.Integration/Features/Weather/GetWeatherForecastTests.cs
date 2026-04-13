using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Extensions;
using Scaffold.Tests.Integration.Shared;
using Scaffold.Weather.Features.GetWeatherForecast;
using System.Net;
using Xunit;

namespace Scaffold.Tests.Integration.Features.Weather;

[Collection(WeatherCollection.Name)]
public sealed class GetWeatherForecastTests(WeatherCollection collection, ITestOutputHelper output)
    : IntegrationTestBase<Program>(collection)
{
    [Fact]
    public async Task should_return_empty_array_when_no_forecasts_exist()
    {
        var result = await AlbaHost.Scenario(s =>
        {
            s.Get.Url("/api/weatherforecast");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output);

        var forecasts = await result.ReadAsJsonAsync<GetWeatherForecastResponse[]>();

        Assert.NotNull(forecasts);
        Assert.Empty(forecasts);
    }

    [Fact]
    public async Task should_return_forecasts_sorted_by_date()
    {
        // Create two forecasts via the API so we have data to verify ordering.
        await AlbaHost.Scenario(s => s.Post.Json(new { }).ToUrl("/api/weatherforecast"));
        await AlbaHost.Scenario(s => s.Post.Json(new { }).ToUrl("/api/weatherforecast"));

        var result = await AlbaHost.Scenario(s =>
        {
            s.Get.Url("/api/weatherforecast");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output, "Sorted forecasts:");

        var forecasts = await result.ReadAsJsonAsync<GetWeatherForecastResponse[]>();

        Assert.NotNull(forecasts);
        Assert.Equal(2, forecasts.Length);
        Assert.True(forecasts[0].Date <= forecasts[1].Date);
    }
}
