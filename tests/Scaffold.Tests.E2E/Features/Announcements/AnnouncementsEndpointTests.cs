using BuildingBlocks.Tests.E2E;
using BuildingBlocks.Tests.E2E.Extensions;
using Scaffold.Tests.E2E.Shared;
using System.Net.Http.Json;
using Xunit;

namespace Scaffold.Tests.E2E.Features.Announcements;

[Collection(ScaffoldCollection.Name)]
public sealed class AnnouncementsEndpointTests(ScaffoldEnvironment environment, ITestOutputHelper output)
    : EndToEndTestBase<Projects.Scaffold_AppHost>(environment)
{
  [Fact]
  public async Task should_create_and_return_announcements_through_gateway_api_route()
  {
    using var httpClient = CreateHttpsClient();

    var createResponse = await httpClient.PostAsJsonAsync(
        "/api/announcements",
        new
        {
          Title = "Gateway announcement",
          Content = "Created through the e2e gateway test."
        },
        TestContext.Current.CancellationToken).PrintBody(output, "Created announcement:");
    var createdContent = await createResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
    Assert.Contains("title", createdContent);
    Assert.Contains("content", createdContent);

    var response = await httpClient.GetAsync(
        "/api/announcements",
        TestContext.Current.CancellationToken).PrintBody(output, "Announcements list:");
    var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.StartsWith("[", content.TrimStart(), StringComparison.Ordinal);
    Assert.Contains("Gateway announcement", content);
    Assert.Contains("Created through the e2e gateway test.", content);
  }
}