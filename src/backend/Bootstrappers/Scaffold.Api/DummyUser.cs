using BuildingBlocks.Core.Interfaces;

namespace Scaffold.Api;

public sealed class DummyUser : ICurrentUser
{
    private static readonly string[] DummyAccess = ["*"];

    public Guid Id { get; } = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public string Email { get; } = "dummy.user@scaffold.local";

    public bool IsAuthenticated => true;

    public IReadOnlyCollection<string> Roles => DummyAccess;

    public IReadOnlyCollection<string> Permissions => DummyAccess;

    public bool HasPermission(string permission)
    {
        return DummyAccess.Contains("*") || Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
    }

    public bool HasRole(string role)
    {
        return DummyAccess.Contains("*") || Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}