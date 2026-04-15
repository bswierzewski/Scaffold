using BuildingBlocks.Tests.E2E;
using BuildingBlocks.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scaffold.Tests.E2E.Shared;

/// <summary>
/// Shared runtime environment for the full Scaffold Aspire stack used by end-to-end tests.
/// </summary>
public sealed class ScaffoldEnvironment : EndToEndTestEnvironment<Projects.Scaffold_AppHost>
{
    /// <summary>
    /// Uses the gateway as the default HTTPS entry point for end-to-end tests.
    /// </summary>
    protected override string DefaultHttpsResourceName => "gateway";

    /// <summary>   
    /// Loads environment variables from envs
    /// </summary>
    protected override void LoadEnvironment() => EnvLoader.LoadEndToEnd(AppContext.BaseDirectory);

    /// <summary>
    /// Configures logging and HTTP client defaults for the Aspire test host.
    /// </summary>
    protected override void ConfigureEnvironmentServices(IServiceCollection services)
    {
        services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });

        services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });
    }

    /// <summary>
    /// Waits until all resources required by the end-to-end tests are healthy.
    /// </summary>
    protected override async ValueTask InitializeEnvironmentAsync()
    {
        await Task.WhenAll(
                WaitForResourceHealthyAsync("scaffold"),
                WaitForResourceHealthyAsync("app"),
                WaitForResourceHealthyAsync("api"),
                WaitForResourceHealthyAsync("gateway"))
            .WaitAsync(TimeSpan.FromMinutes(3));
    }
}