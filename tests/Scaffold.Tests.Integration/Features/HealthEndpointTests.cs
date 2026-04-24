using System.Net;
using Scaffold.Tests.Integration.Shared;

namespace Scaffold.Tests.Integration.Features;

[Collection(ScaffoldCollection.Name)]
public sealed class HealthEndpointTests(ScaffoldEnvironment environment)
{
    [Fact]
    public async Task Health_endpoint_returns_success()
    {
        using var client = environment.Host.Server.CreateClient();

        using var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}