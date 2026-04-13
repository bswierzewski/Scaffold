using Microsoft.EntityFrameworkCore;
using Scaffold.Announcements.Infrastructure.Persistence;
using Wolverine.Http;

namespace Scaffold.Announcements.Features.GetAnnouncements;

public static class GetAnnouncementsHandler
{
  [WolverineGet("/api/announcements")]
  public static async Task<GetAnnouncementsResponse[]> Handle(
      AnnouncementsDbContext dbContext,
      CancellationToken cancellationToken)
  {
    return await dbContext.Announcements
        .AsNoTracking()
        .OrderByDescending(x => x.PublishedAt)
        .Select(x => new GetAnnouncementsResponse(
            x.Id,
            x.Title,
            x.Content,
            x.PublishedAt))
        .ToArrayAsync(cancellationToken);
  }
}