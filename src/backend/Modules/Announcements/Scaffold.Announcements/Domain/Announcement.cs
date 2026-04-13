using BuildingBlocks.Core.Primitives;

namespace Scaffold.Announcements.Domain;

public sealed class Announcement : AuditableEntity<Guid>
{
  private Announcement() { }

  public string Title { get; private set; } = string.Empty;

  public string Content { get; private set; } = string.Empty;

  public DateTimeOffset PublishedAt { get; private set; }

  public static Announcement Create(string title, string content)
  {
    return new Announcement
    {
      Id = Guid.NewGuid(),
      Title = title.Trim(),
      Content = content.Trim(),
      PublishedAt = DateTimeOffset.UtcNow
    };
  }
}