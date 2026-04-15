using BuildingBlocks.Tests.Authentication.Jwt;
using BuildingBlocks.Tests.Authentication.Users;
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
  public async Task should_return_unauthorized_when_creating_without_token()
  {
    using var httpClient = CreateHttpsClient();

    using var response = await httpClient.PostAsJsonAsync(
        "/api/announcements",
        new
        {
          Title = "Gateway announcement",
          Content = "Created through the e2e gateway test."
        },
        TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task should_return_forbidden_when_creating_as_regular_user()
  {
    using var httpClient = CreateHttpsClient().AuthenticateWith(TestUsers.User);

    using var response = await httpClient.PostAsJsonAsync(
        "/api/announcements",
        new
        {
          Title = "Gateway announcement",
          Content = "Created through the e2e gateway test."
        },
        TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
  }

  [Fact]
  public async Task should_create_announcement_when_authenticated_as_admin()
  {
    using var httpClient = CreateHttpsClient().AuthenticateWith(TestUsers.Admin);

    using var response = await httpClient.PostAsJsonAsync(
        "/api/announcements",
        new
        {
          Title = "Gateway announcement",
          Content = "Created through the e2e gateway test."
        },
        TestContext.Current.CancellationToken);

    var payload = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Contains("Gateway announcement", payload);
  }

  [Fact]
  public async Task should_return_announcements_list_through_gateway_api_route()
  {
    using var httpClient = CreateHttpsClient();

    var response = await httpClient.GetAsync(
        "/api/announcements",
        TestContext.Current.CancellationToken).PrintBody(output, "Announcements list:");
    var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.StartsWith("[", content.TrimStart(), StringComparison.Ordinal);
  }
}