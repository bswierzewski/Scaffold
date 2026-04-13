namespace Scaffold.Announcements.Features.GetAnnouncements;

public sealed record GetAnnouncementsResponse(
    Guid Id,
    string Title,
    string Content,
    DateTimeOffset PublishedAt);