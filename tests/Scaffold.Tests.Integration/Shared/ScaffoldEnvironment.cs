using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
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
    private const string DefaultConnectionStringEnvironmentVariable = "ConnectionStrings__Default";

    private PostgreSqlContainer _database = default!;
    private Respawner _respawner = default!;
    private NpgsqlConnection _resetConnection = default!;

    public WebApplicationFactory<Program> Factory { get; private set; } = default!;

    public IServiceProvider Services => Factory.Services;

    public string ConnectionString => _database.GetConnectionString();

    /// <summary>
    /// Creates an HTTP client bound to the in-memory API host.
    /// </summary>
    public HttpClient CreateClient()
        => Factory.CreateClient();

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
            .Build();

        await _database.StartAsync();

        Environment.SetEnvironmentVariable(DefaultConnectionStringEnvironmentVariable, _database.GetConnectionString());

        Factory = new IntegrationTestWebApplicationFactory(_database.GetConnectionString());

        _ = CreateClient();

        _resetConnection = new NpgsqlConnection(_database.GetConnectionString());
        await _resetConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_resetConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            TablesToIgnore =
            [
                new Table("identity", "__EFMigrationsHistory")
            ]
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
        if (Factory is not null)
            await Factory.DisposeAsync();

        if (_resetConnection is not null)
            await _resetConnection.DisposeAsync();

        if (_database is not null)
            await _database.DisposeAsync();

        Environment.SetEnvironmentVariable(DefaultConnectionStringEnvironmentVariable, null);
    }

    private sealed class IntegrationTestWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:Default"] = connectionString
                });
            });
        }
    }
}