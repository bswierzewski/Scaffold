using BuildingBlocks.Tests.Integration;
using BuildingBlocks.Tests.Integration.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for the full integration-test stack.
/// Applies module schemas before Respawn inspects database tables.
/// </summary>
public sealed class ScaffoldEnvironment : IntegrationTestEnvironment<Program>
{
    public override void ConfigureServices(IServiceCollection services)
    {
    }

    protected override async ValueTask InitializeEnvironmentAsync()
    {
    }
}