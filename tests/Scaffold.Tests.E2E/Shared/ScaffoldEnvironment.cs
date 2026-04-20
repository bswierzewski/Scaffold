using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Scaffold.Tests.E2E.Shared;

/// <summary>
/// Shared runtime environment for the full Scaffold Aspire stack used by end-to-end tests.
/// </summary>
public sealed class ScaffoldEnvironment : IAsyncLifetime
{
    private const string GatewayResourceName = "gateway";

    public DistributedApplication App { get; private set; } = default!;

    /// <summary>
    /// Main HTTPS entry point used by end-to-end tests.
    /// </summary>
    public HttpClient CreateHttpsClient(string? resourceName = null)
        => App.CreateHttpClient(resourceName ?? GatewayResourceName, "https");

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
                App.ResourceNotifications.WaitForResourceHealthyAsync(GatewayResourceName))
            .WaitAsync(TimeSpan.FromMinutes(3));
    }

    /// <summary>
    /// Disposes the distributed application created for the test collection.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (App is not null)
            await App.DisposeAsync();
    }
}