using BuildingBlocks.Core.Abstractions;

namespace Scaffold.Api.Authentication;

/// <summary>
/// A dummy implementation of ICurrentUser for demonstration purposes.  
/// In a real application, this would be replaced with an implementation that retrieves user information from the authentication context.
/// </summary>
internal sealed class DummyUserContext : ICurrentUser
{
  public Guid Id => Guid.Parse("00000000-0000-0000-0000-000000000001");

  public IEnumerable<string> Roles => ["User", "Admin"];

  public bool IsInRole(string role)
      => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

}