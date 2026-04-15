using BuildingBlocks.Core.Attributes;
using Scaffold.Announcements.Domain;
using Scaffold.Announcements.Features.GetAnnouncements;
using Scaffold.Announcements.Infrastructure.Persistence;
using Wolverine.Http;

namespace Scaffold.Announcements.Features.CreateAnnouncement;

public sealed record CreateAnnouncementRequest(
    string Title,
    string Content);

public static class CreateAnnouncementHandler
{
  [Authorize(Roles = "admin")]
  [WolverinePost("/api/announcements")]
  public static async Task<GetAnnouncementsResponse> Handle(
      CreateAnnouncementRequest request,
      AnnouncementsDbContext dbContext,
      CancellationToken cancellationToken)
  {
    var announcement = Announcement.Create(request.Title, request.Content);

    dbContext.Announcements.Add(announcement);
    await dbContext.SaveChangesAsync(cancellationToken);

    return new GetAnnouncementsResponse(
        announcement.Id,
        announcement.Title,
        announcement.Content,
        announcement.PublishedAt);
  }
}