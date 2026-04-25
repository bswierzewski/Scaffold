namespace Scaffold.Modules.Notifications.Features.CreateNotificationMessage;

public sealed record CreateNotificationMessageCommand(
    string Title,
    string Body);

public sealed record CreateNotificationMessageResponse(Guid Id);