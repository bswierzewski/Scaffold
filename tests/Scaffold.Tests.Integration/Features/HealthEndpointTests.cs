using System.Net;
using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Fixtures;
using Scaffold.Tests.Integration.Shared;

namespace Scaffold.Tests.Integration.Features;

[Collection(ScaffoldCollection.Name)]
public sealed class HealthEndpointTests(DatabaseFixture databaseFixture)
    : IntegrationTestBase<Program>(databaseFixture)
{
    [Fact]
    public async Task Health_endpoint_returns_success()
    {
        using var client = Host.Server.CreateClient();

        using var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}