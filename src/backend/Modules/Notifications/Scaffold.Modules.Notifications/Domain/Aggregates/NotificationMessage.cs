using BuildingBlocks.Core.Primitives;

namespace Scaffold.Modules.Notifications.Domain.Aggregates;

public sealed class NotificationMessage : AuditableEntity<Guid>
{
    public string Title { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;

    private NotificationMessage() { }

    private NotificationMessage(Guid id, string title, string body)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);

        Id = id;
        Title = title.Trim();
        Body = body.Trim();
    }

    public static NotificationMessage Create(string title, string body)
        => new(Guid.CreateVersion7(), title, body);
}