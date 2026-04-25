namespace Scaffold.Modules.Notifications.Features.GetNotificationMessages;

public sealed record GetNotificationMessagesResponse(
    Guid Id,
    string Title,
    string Body);