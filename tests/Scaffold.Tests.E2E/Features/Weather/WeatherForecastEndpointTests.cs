using BuildingBlocks.Tests.E2E;
using BuildingBlocks.Tests.E2E.Extensions;
using Scaffold.Tests.E2E.Shared;
using System.Net.Http.Json;

namespace Scaffold.Tests.E2E.Features.Weather;

[Collection(ScaffoldCollection.Name)]
public sealed class WeatherForecastEndpointTests(ScaffoldEnvironment environment, ITestOutputHelper output)
    : EndToEndTestBase<Projects.Scaffold_AppHost, ScaffoldEnvironment>(environment)
{
    [Fact]
    public async Task should_return_unauthorized_when_creating_without_token()
    {
        using var httpClient = Environment.App.CreateHttpClient(Environment.GatewayResourceName, "https");

        using var response = await httpClient.PostAsJsonAsync(
            "/api/weatherforecast",
            new { },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task should_return_forbidden_when_creating_as_regular_user()
    {
        using var httpClient = Environment.App.CreateHttpClient(Environment.GatewayResourceName, "https").As(Users.User);

        using var response = await httpClient.PostAsJsonAsync(
            "/api/weatherforecast",
            new { },
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task should_create_weather_forecast_when_authenticated_as_admin()
    {
        using var httpClient = Environment.App.CreateHttpClient(Environment.GatewayResourceName, "https").As(Users.Admin);

        using var response = await httpClient.PostAsJsonAsync(
            "/api/weatherforecast",
            new { },
            TestContext.Current.CancellationToken);

        var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("temperatureC", payload);
        Assert.Contains("date", payload);
    }

    [Fact]
    public async Task should_return_weather_forecast_list_through_gateway_api_route()
    {
        using var httpClient = Environment.App.CreateHttpClient(Environment.GatewayResourceName, "https");

        var response = await httpClient.GetAsync(
            "/api/weatherforecast",
            TestContext.Current.CancellationToken).PrintBody(output, "Forecast list:");
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.StartsWith("[", content.TrimStart(), StringComparison.Ordinal);
        Assert.Contains("temperatureC", content);
        Assert.Contains("date", content);
    }
}