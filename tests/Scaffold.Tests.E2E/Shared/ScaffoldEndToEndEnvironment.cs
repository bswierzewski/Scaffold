using BuildingBlocks.Tests.E2E;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Scaffold.Tests.E2E.Shared;

public sealed class ScaffoldEndToEndEnvironment : EndToEndTestEnvironment<Projects.Scaffold_AppHost>
{
    protected override string DefaultHttpsResourceName => "gateway";

    protected override ValueTask ConfigureTestingServicesAsync(IServiceCollection services)
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

        return ValueTask.CompletedTask;
    }

    protected override async ValueTask OnApplicationStartedAsync()
    {
        await Task.WhenAll(
                WaitForResourceHealthyAsync("scaffold"),
                WaitForResourceHealthyAsync("app"),
                WaitForResourceHealthyAsync("api"),
                WaitForResourceHealthyAsync("gateway"))
            .WaitAsync(TimeSpan.FromMinutes(3));
    }
}