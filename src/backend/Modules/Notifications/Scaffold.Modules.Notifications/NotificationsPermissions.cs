using BuildingBlocks.Core.Primitives;

namespace Scaffold.Modules.Notifications;

public static class NotificationsPermissions
{
    public static readonly Permission ReadMessages = new("notifications.messages.read", "Read notification messages");
    public static readonly Permission ManageMessages = new("notifications.messages.write", "Manage notification messages");
}