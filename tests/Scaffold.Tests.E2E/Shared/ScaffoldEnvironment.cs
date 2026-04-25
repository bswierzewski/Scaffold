using Aspire.Hosting;
using BuildingBlocks.Tests.Shared;
using Scaffold.AppHost;

namespace Scaffold.Tests.E2E.Shared;

/// <summary>
/// Shared runtime environment for the full Scaffold Aspire stack used by end-to-end tests.
/// </summary>
public sealed class ScaffoldEnvironment : IAsyncLifetime
{
    private readonly DatabaseRespawner _databaseRespawner = new();

    /// <summary>
    /// The started distributed application used by end-to-end tests.
    /// </summary>
    public DistributedApplication App { get; private set; } = default!;

    /// <summary>
    /// HTTP client targeting the gateway resource.
    /// </summary>
    public HttpClient GatewayHttpClient { get; private set; } = default!;

    // Lifecycle

    /// <summary>
    /// Builds and starts the distributed application used by this test collection.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Scaffold_AppHost>();

        App = await builder.BuildAsync();
        await App.StartAsync();

        await Task.WhenAll(
                App.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Database),
                App.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.App),
                App.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Api),
                App.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Gateway)
            )
            .WaitAsync(TimeSpan.FromMinutes(3));

        GatewayHttpClient = App.CreateHttpClient(ResourceNames.Gateway, "https");

        await _databaseRespawner.InitializeAsync(await App.GetConnectionStringAsync(ResourceNames.Database));
    }

    /// <summary>
    /// Disposes the distributed application created for the test collection.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (GatewayHttpClient is not null)
            GatewayHttpClient.Dispose();

        await _databaseRespawner.DisposeAsync();

        if (App is not null)
            await App.DisposeAsync();
    }

    /// <summary>
    /// Resets the database to a clean state while preserving the applied migration history.
    /// </summary>
    public Task ResetDatabaseAsync()
        => _databaseRespawner.ResetAsync();
}