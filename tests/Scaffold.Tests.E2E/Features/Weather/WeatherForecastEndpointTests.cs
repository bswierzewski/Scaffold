using BuildingBlocks.Tests.E2E;
using Scaffold.Tests.E2E.Shared;
using System.Net.Http.Json;
using Xunit;

namespace Scaffold.Tests.E2E.Features.Weather;

[Collection(ScaffoldCollection.Name)]
public sealed class WeatherForecastEndpointTests(ScaffoldEndToEndEnvironment environment)
    : EndToEndTestBase<Projects.Scaffold_AppHost>(environment)
{
    [Fact]
    public async Task should_create_and_return_weather_forecast_through_gateway_api_route()
    {
        using var httpClient = CreateHttpsClient();

        var createResponse = await httpClient.PostAsJsonAsync(
            "/api/weatherforecast",
            new { },
            TestContext.Current.CancellationToken);
        var createdContent = await createResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.Contains("temperatureC", createdContent);
        Assert.Contains("date", createdContent);

        var response = await httpClient.GetAsync("/api/weatherforecast", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.StartsWith("[", content.TrimStart(), StringComparison.Ordinal);
        Assert.Contains("temperatureC", content);
        Assert.Contains("date", content);
    }
}