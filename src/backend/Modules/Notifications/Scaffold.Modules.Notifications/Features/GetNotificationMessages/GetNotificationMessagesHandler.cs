using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Scaffold.Modules.Notifications.Infrastructure.Persistence;
using Wolverine.Http;

namespace Scaffold.Modules.Notifications.Features.GetNotificationMessages;

public sealed class GetNotificationMessagesHandler
{
    [WolverineGet("/notifications/messages")]
    [Tags("Notifications")]
    [EndpointName("GetNotificationMessages")]
    [EndpointSummary("Get all notification messages")]
    public static async Task<List<GetNotificationMessagesResponse>> Handle(
        NotificationsDbContext dbContext,
        CancellationToken ct)
    {
        return await dbContext.NotificationMessages
            .AsNoTracking()
            .OrderBy(message => message.Title)
            .Select(message => new GetNotificationMessagesResponse(
                message.Id,
                message.Title,
                message.Body))
            .ToListAsync(ct);
    }
}