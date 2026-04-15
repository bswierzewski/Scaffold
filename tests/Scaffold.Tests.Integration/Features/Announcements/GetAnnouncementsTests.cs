using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Extensions;
using Scaffold.Announcements.Features.GetAnnouncements;
using Scaffold.Tests.Integration.Shared;

namespace Scaffold.Tests.Integration.Features.Announcements;

[Collection(ScaffoldCollection.Name)]
public sealed class GetAnnouncementsTests(ScaffoldEnvironment environment, ITestOutputHelper output)
    : IntegrationTestBase<Program>(environment)
{
    [Fact]
    public async Task should_return_empty_array_when_no_announcements_exist()
    {
        var result = await AlbaHost.Scenario(s =>
        {
            s.Get.Url("/api/announcements");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output);

        var announcements = await result.ReadAsJsonAsync<GetAnnouncementsResponse[]>();

        Assert.NotNull(announcements);
        Assert.Empty(announcements);
    }

    [Fact]
    public async Task should_return_announcements_sorted_by_published_at_descending()
    {
        await AlbaHost.Scenario(s => s.Post.Json(new
        {
            Title = "Earlier message",
            Content = "Created first"
        }).ToUrl("/api/announcements"));

        await Task.Delay(25, TestContext.Current.CancellationToken);

        await AlbaHost.Scenario(s => s.Post.Json(new
        {
            Title = "Later message",
            Content = "Created second"
        }).ToUrl("/api/announcements"));

        var result = await AlbaHost.Scenario(s =>
        {
            s.Get.Url("/api/announcements");
            s.StatusCodeShouldBe(HttpStatusCode.OK);
        }).PrintBody(output, "Sorted announcements:");

        var announcements = await result.ReadAsJsonAsync<GetAnnouncementsResponse[]>();

        Assert.NotNull(announcements);
        Assert.Equal(2, announcements.Length);
        Assert.True(announcements[0].PublishedAt >= announcements[1].PublishedAt);
    }
}