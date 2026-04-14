using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Serilog.Extensions;
using Microsoft.Extensions.Hosting;
using Scaffold.Bootstrapping;

var modules = ScaffoldModules.Create();

var builder = Host.CreateDefaultBuilder(args)
    .UseSerilog(serilog =>
    {
        serilog
            .AddFile()
            .AddConsole()
            .AddOpenTelemetry();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddPostgresDataSource(context.Configuration);

        foreach (var module in modules)
            module.AddServices(services, context.Configuration);
    });

using var host = builder.Build();

foreach (var module in modules)
    await module.InitializeMigrationsAsync(host.Services);
