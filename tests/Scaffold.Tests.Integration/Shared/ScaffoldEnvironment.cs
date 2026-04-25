using Alba;
using BuildingBlocks.Tests.Integration.Containers;
using BuildingBlocks.Tests.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Scaffold.Tests.Integration.Shared;

/// <summary>
/// Shared runtime environment for API integration tests backed by a PostgreSQL Testcontainer.
/// </summary>
public class ScaffoldEnvironment : IAsyncLifetime
{
    private readonly PostgresTestContainer _database = new();
    private readonly DatabaseRespawner _databaseRespawner = new();

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
            builder.ConfigureServices((_, services) => ConfigureTestServices(services));
        });

        await _databaseRespawner.InitializeAsync(_database.ConnectionString);
    }

    /// <summary>
    /// Disposes the API host, reset connection and PostgreSQL container.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (Host is not null)
            await Host.DisposeAsync();

        await _databaseRespawner.DisposeAsync();

        await _database.DisposeAsync();
    }

    /// <summary>
    /// Resets the database to a clean state while preserving the applied migration history.
    /// </summary>
    public Task ResetDatabaseAsync()
        => _databaseRespawner.ResetAsync();

    /// <summary>
    /// Allows derived fixtures to replace or add service registrations before the host is built.
    /// </summary>
    protected virtual void ConfigureTestServices(IServiceCollection services)
    {
    }
}