using BuildingBlocks.Tests.E2E;
using Scaffold.Tests.E2E.Shared;
using Xunit;

namespace Scaffold.Tests.E2E.Features.Gateway;

[Collection(ScaffoldCollection.Name)]
public sealed class GatewayRootTests(ScaffoldEndToEndEnvironment environment)
    : EndToEndTestBase<Projects.Scaffold_AppHost>(environment)
{
    [Fact]
    public async Task should_return_frontend_html_from_gateway_root()
    {
        using var httpClient = CreateHttpClient("gateway", ScaffoldEndToEndEnvironment.GatewayHttpsEndpoint);

        var response = await httpClient.GetAsync("/", TestContext.Current.CancellationToken);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("<!doctype html>", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<html", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("<body", content, StringComparison.OrdinalIgnoreCase);
    }
}