using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Xunit;

namespace Scaffold.Tests.E2E.Shared;

/// <summary>
/// Shared runtime environment for the full Scaffold Aspire stack used by end-to-end tests.
/// </summary>
public sealed class ScaffoldEnvironment : IAsyncLifetime
{
    private Respawner _respawner = default!;
    private NpgsqlConnection _resetConnection = default!;

    public DistributedApplication App { get; private set; } = default!;

    /// <summary>
    /// Builds and starts the distributed application used by this test collection.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Scaffold_AppHost>();

        builder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        builder.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        App = await builder.BuildAsync();
        await App.StartAsync();

        await Task.WhenAll(
                App.ResourceNotifications.WaitForResourceHealthyAsync("db"),
                App.ResourceNotifications.WaitForResourceHealthyAsync("app"),
                App.ResourceNotifications.WaitForResourceHealthyAsync("api"),
                App.ResourceNotifications.WaitForResourceHealthyAsync("gateway"))
            .WaitAsync(TimeSpan.FromMinutes(3));

        var connectionString = App.GetConnectionString("db")
            ?? throw new InvalidOperationException("Connection string for 'db' resource was not found.");

        _resetConnection = new NpgsqlConnection(connectionString);
        await _resetConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_resetConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = ["__EFMigrationsHistory"]
        });
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
        if (_resetConnection is not null)
            await _resetConnection.DisposeAsync();

        if (App is not null)
            await App.DisposeAsync();
    }
}