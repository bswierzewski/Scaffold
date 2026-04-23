using Alba;
using Npgsql;
using Respawn;
using Respawn.Graph;
using Testcontainers.PostgreSql;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for API integration tests backed by an ephemeral PostgreSQL container.
/// </summary>
public sealed class ScaffoldEnvironment : IAsyncLifetime
{

    private PostgreSqlContainer _database = default!;
    private Respawner _respawner = default!;
    private NpgsqlConnection _resetConnection = default!;
    private IAlbaHost _host = default!;

    public IServiceProvider Services => _host.Services;

    /// <summary>
    /// Creates an HTTP client bound to the in-memory API host.
    /// </summary>
    public HttpClient CreateClient()
        => _host.Server.CreateClient();

    /// <summary>
    /// Starts the PostgreSQL container, boots the API and prepares Respawn for fast database resets.
    /// </summary>
    public async ValueTask InitializeAsync()
    {
        _database = new PostgreSqlBuilder()
            .WithImage("postgres:18-alpine")
            .WithDatabase("scaffold_integration")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, 5432)
            .Build();

        await _database.StartAsync();

        _host = await AlbaHost.For<Program>();

        _ = CreateClient();

        _resetConnection = new NpgsqlConnection(ConnectionString);
        await _resetConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_resetConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore = [ "__EFMigrationsHistory" ] 
        });
    }

    /// <summary>
    /// Resets the database to a clean state while preserving the applied migration history.
    /// </summary>
    public Task ResetDatabaseAsync()
        => _respawner.ResetAsync(_resetConnection);

    /// <summary>
    /// Disposes the API host, reset connection and PostgreSQL container.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_host is not null)
            await _host.DisposeAsync();

        if (_resetConnection is not null)
            await _resetConnection.DisposeAsync();

        if (_database is not null)
            await _database.DisposeAsync();
    }
}