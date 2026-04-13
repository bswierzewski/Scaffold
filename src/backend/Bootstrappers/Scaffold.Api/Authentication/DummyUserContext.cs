using BuildingBlocks.Core.Abstractions;

namespace Scaffold.Api.Authentication;

/// <summary>
/// A dummy implementation of ICurrentUser for demonstration purposes.  
/// In a real application, this would be replaced with an implementation that retrieves user information from the authentication context.
/// </summary>
internal sealed class DummyUserContext : ICurrentUser
{
  public string Id => "user_000000000000000000000001";

  public IEnumerable<string> Roles => ["User", "Admin"];

  public bool IsInRole(string role)
      => Roles.Contains(role, StringComparer.OrdinalIgnoreCase);

}