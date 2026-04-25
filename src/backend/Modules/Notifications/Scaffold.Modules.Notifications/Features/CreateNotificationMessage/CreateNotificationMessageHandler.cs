using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Scaffold.Modules.Notifications.Domain.Aggregates;
using Scaffold.Modules.Notifications.Infrastructure.Persistence;
using Scaffold.Modules.Notifications.Infrastructure.Services;
using Wolverine.Http;

namespace Scaffold.Modules.Notifications.Features.CreateNotificationMessage;

public sealed class CreateNotificationMessageHandler
{
    [WolverinePost("/api/notifications/messages")]
    [Tags("Notifications")]
    [EndpointName("CreateNotificationMessage")]
    [EndpointSummary("Create a notification message")]
    public static async Task<CreateNotificationMessageResponse> Handle(
        CreateNotificationMessageCommand command,
        NotificationsDbContext dbContext,
        IPrinter printer,
        CancellationToken ct)
    {
        var message = NotificationMessage.Create(command.Title, command.Body);

        await dbContext.NotificationMessages.AddAsync(message, ct);

        printer.Print($"[Notifications] Created message '{message.Title}'.");

        return new CreateNotificationMessageResponse(message.Id);
    }
}