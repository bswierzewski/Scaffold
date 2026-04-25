using Aspire.Hosting;
using Npgsql;
using Respawn;
using Scaffold.AppHost;

namespace Scaffold.Tests.E2E.Shared;

/// <summary>
/// Shared runtime environment for the full Scaffold Aspire stack used by end-to-end tests.
/// </summary>
public sealed class ScaffoldEnvironment : IAsyncLifetime
{
    // Runtime state
    private Respawner _respawner = default!;
    private NpgsqlConnection _resetConnection = default!;

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
            App.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Gateway))
            .WaitAsync(TimeSpan.FromMinutes(3));

        InitializeHttpsClients();

        await InitializeDatabaseConnectionAsync();
        await InitializeRespawnerAsync();
    }

    /// <summary>
    /// Resets the database to a clean state while preserving the applied migration history.
    /// </summary>
    public Task ResetDatabaseAsync()
        => _respawner.ResetAsync(_resetConnection);

    /// <summary>
    /// Disposes the distributed application created for the test collection.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (GatewayHttpClient is not null)
            GatewayHttpClient.Dispose();

        if (_resetConnection is not null)
            await _resetConnection.DisposeAsync();

        if (App is not null)
            await App.DisposeAsync();
    }

    // Initialization helpers

    /// <summary>
    /// Creates the HTTPS client used to call the gateway during end-to-end tests.
    /// </summary>
    private void InitializeHttpsClients()
    {
        GatewayHttpClient = App.CreateHttpClient(ResourceNames.Gateway, "https");
    }

    /// <summary>
    /// Opens a database connection using the connection string exposed by the Aspire database resource.
    /// </summary>
    private async Task InitializeDatabaseConnectionAsync()
    {
        var connectionString = await App.GetConnectionStringAsync(ResourceNames.Database)
            ?? throw new InvalidOperationException($"Connection string for '{ResourceNames.Database}' resource was not found.");

        _resetConnection = new NpgsqlConnection(connectionString);
        await _resetConnection.OpenAsync();
    }

    /// <summary>
    /// Creates the Respawn checkpoint used to reset the database between tests.
    /// </summary>
    private async Task InitializeRespawnerAsync()
    {
        _respawner = await Respawner.CreateAsync(_resetConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }
}