using BuildingBlocks.Tests.E2E;
using Microsoft.Extensions.Logging;

namespace Scaffold.Tests.E2E.Shared;

/// <summary>
/// Shared runtime environment for the full Scaffold Aspire stack used by end-to-end tests.
/// </summary>
public sealed class ScaffoldEnvironment : EndToEndTestEnvironment<Projects.Scaffold_AppHost>
{
    /// <summary>
    /// Main HTTPS entry point used by end-to-end tests.
    /// </summary>
    public string GatewayResourceName => "gateway";

    /// <summary>
    /// Configures logging and HTTP client defaults for the Aspire test host.
    /// </summary>
    protected override void ConfigureServices(IServiceCollection services)
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
                App.ResourceNotifications.WaitForResourceHealthyAsync("db"),
                App.ResourceNotifications.WaitForResourceHealthyAsync("app"),
                App.ResourceNotifications.WaitForResourceHealthyAsync("api"),
                App.ResourceNotifications.WaitForResourceHealthyAsync(GatewayResourceName))
            .WaitAsync(TimeSpan.FromMinutes(3));
    }
}