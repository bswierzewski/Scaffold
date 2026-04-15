using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Extensions;
using Scaffold.Announcements.Features.GetAnnouncements;
using Scaffold.Tests.Integration.Shared;

namespace Scaffold.Tests.Integration.Features.Announcements;

[Collection(ScaffoldCollection.Name)]
public sealed class CreateAnnouncementTests(ScaffoldEnvironment environment, ITestOutputHelper output)
    : IntegrationTestBase<Program>(environment)
{
    [Fact]
    public async Task should_create_announcement_and_return_valid_response()
    {
        var request = new
        {
            Title = "Scheduled maintenance",
            Content = "The platform will be unavailable for 30 minutes."
        };

        var result = await AlbaHost.Scenario(s =>
        {
            s.As(Users.Admin);
            s.Post.Json(request).ToUrl("/api/announcements");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output, "Created announcement:");

        var announcement = await result.ReadAsJsonAsync<GetAnnouncementsResponse>();

        Assert.NotNull(announcement);
        Assert.NotEqual(Guid.Empty, announcement.Id);
        Assert.Equal(request.Title, announcement.Title);
        Assert.Equal(request.Content, announcement.Content);
        Assert.NotEqual(default, announcement.PublishedAt);
    }

    [Fact]
    public async Task created_announcement_should_appear_in_get_results()
    {
        var request = new
        {
            Title = "New feature rollout",
            Content = "Announcements module is now available."
        };

        var createResult = await AlbaHost.Scenario(s =>
        {
            s.As(Users.Admin);
            s.Post.Json(request).ToUrl("/api/announcements");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var created = await createResult.ReadAsJsonAsync<GetAnnouncementsResponse>();
        Assert.NotNull(created);

        var listResult = await AlbaHost.Scenario(s =>
        {
            s.Get.Url("/api/announcements");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output, "Announcements after create:");

        var announcements = await listResult.ReadAsJsonAsync<GetAnnouncementsResponse[]>();

        Assert.NotNull(announcements);
        Assert.Single(announcements);
        Assert.Equal(created.Id, announcements[0].Id);
        Assert.Equal(created.Title, announcements[0].Title);
        Assert.Equal(created.Content, announcements[0].Content);
    }
}