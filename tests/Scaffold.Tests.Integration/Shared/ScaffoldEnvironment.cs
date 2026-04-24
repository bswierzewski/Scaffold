using Alba;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Scaffold.AppHost;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for API integration tests backed by an Aspire-managed PostgreSQL database.
/// </summary>
public sealed class ScaffoldEnvironment : IAsyncLifetime
{
    private Respawner _respawner = default!;
    private NpgsqlConnection _resetConnection = default!;

    public DistributedApplication App { get; private set; } = default!;
    public IAlbaHost Host { get; private set; } = default!;

    /// <summary>
    /// Starts the Aspire-managed PostgreSQL database, boots the API and prepares Respawn for fast database resets.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Scaffold_AppHost>(
            [
                "Tests:DatabaseOnly=true"
            ]);

        App = await builder.BuildAsync();
        await App.StartAsync();

        await App.ResourceNotifications.WaitForResourceHealthyAsync(ResourceNames.Database)
            .WaitAsync(TimeSpan.FromMinutes(3));

        await InitializeDatabaseConnectionAsync();

        Host = await AlbaHost.For<Scaffold.Api.Program>();

        await InitializeRespawnerAsync();
    }

    /// <summary>
    /// Resets the database to a clean state while preserving the applied migration history.
    /// </summary>
    public Task ResetDatabaseAsync()
        => _respawner.ResetAsync(_resetConnection);

    /// <summary>
    /// Disposes the API host, reset connection and Aspire database application.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Host is not null)
            await Host.DisposeAsync();

        if (_resetConnection is not null)
            await _resetConnection.DisposeAsync();

        if (App is not null)
            await App.DisposeAsync();
    }

    private async Task InitializeDatabaseConnectionAsync()
    {
        var connectionString = await App.GetConnectionStringAsync(ResourceNames.Database)
            ?? throw new InvalidOperationException($"Connection string for '{ResourceNames.Database}' resource was not found.");

        Environment.SetEnvironmentVariable("ConnectionStrings__Default", connectionString);

        _resetConnection = new NpgsqlConnection(connectionString);
        await _resetConnection.OpenAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
        _respawner = await Respawner.CreateAsync(_resetConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
    }
}