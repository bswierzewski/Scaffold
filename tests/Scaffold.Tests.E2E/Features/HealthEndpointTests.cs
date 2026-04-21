using System.Net;
using Scaffold.Tests.E2E.Shared;

namespace Scaffold.Tests.E2E.Features;

[Collection(ScaffoldCollection.Name)]
public sealed class HealthEndpointTests(ScaffoldEnvironment environment)
{
    [Fact]
    public async Task Health_endpoint_returns_success()
    {
        using var client = environment.App.CreateHttpClient("gateway");

        using var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}