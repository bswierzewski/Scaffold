using Alba;
using BuildingBlocks.Tests.Integration.Containers;
using Npgsql;
using Respawn;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for API integration tests backed by a PostgreSQL Testcontainer.
/// </summary>
public sealed class ScaffoldEnvironment : IAsyncLifetime
{
    private readonly PostgresTestContainer _database = new();
    private Respawner _respawner = default!;
    private NpgsqlConnection _resetConnection = default!;

    public IAlbaHost Host { get; private set; } = default!;

    /// <summary>
    /// Starts the PostgreSQL container, boots the API and prepares Respawn for fast database resets.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        await _database.StartAsync();

        Host = await AlbaHost.For<Program>(builder =>
        {
            builder.UseSetting("ConnectionStrings:Default", _database.ConnectionString);
        });

        _resetConnection = await _database.OpenConnectionAsync();
        await InitializeRespawnerAsync();
    }

    /// <summary>
    /// Disposes the API host, reset connection and PostgreSQL container.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Host is not null)
            await Host.DisposeAsync();

        if (_resetConnection is not null)
            await _resetConnection.DisposeAsync();

        await _database.DisposeAsync();
    }

    private async Task InitializeRespawnerAsync()
    {
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
}