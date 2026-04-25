using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Hosting;
using BuildingBlocks.Identity;
using BuildingBlocks.Infrastructure.Exceptions.Extensions;
using BuildingBlocks.Infrastructure.Exceptions.Handlers;
using BuildingBlocks.Infrastructure.OpenApi;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using BuildingBlocks.Infrastructure.Serilog.Extensions;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Scaffold.Modules.Catalog;
using Scaffold.Modules.Notifications;

var builder = WebApplication.CreateBuilder(args);

// Adds Aspire defaults such as service discovery, health checks, telemetry wiring and resilient defaults
// shared across distributed application services.
builder.AddServiceDefaults();

// Replaces the default logging pipeline with Serilog and enables the sinks we want in this service:
// file logs for local diagnostics, console logs for container/dev output, and OpenTelemetry export.
builder.Host.UseSerilog(serilog =>
{
    serilog
        .AddFile()
        .AddConsole()
        .AddOpenTelemetry();
});

// Registers the global exception handler so unhandled exceptions are converted to one consistent API response shape.
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Enables RFC 7807 ProblemDetails responses and adds diagnostic metadata useful during debugging.
builder.Services.AddProblemDetails(options =>
{
    options.AddDiagnosticInformation();
});

// Registers OpenAPI generation and teaches the document generator about our ProblemDetails responses.
builder.Services.AddOpenApi(options =>
{
    options.AddProblemDetailsResponses();
});

// Lists application modules explicitly so the bootstrapper can register their services
// and expose their Wolverine handlers/endpoints.
IModule[] modules =
[
    new IdentityModule(),
    new CatalogModule(),
    new NotificationsModule()
];

foreach (var module in modules)
    builder.Services.AddSingleton(typeof(IModule), module);

// Let each module register its container services explicitly at the composition root.
foreach (var module in modules)
    module.AddServices(builder.Services, builder.Configuration);

var dataSource = builder.Services.AddPostgresDataSource(builder.Configuration, "Default");

builder.AddWolverine(modules, dataSource);

// Builds the DI container and the HTTP pipeline. After this point service registrations are closed.
var app = builder.Build();

foreach (var module in modules)
    await module.InitializeAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    // Exposes the generated OpenAPI document only in development.
    app.MapOpenApi();
}

// Enables the global exception handler middleware registered earlier.
app.UseExceptionHandler();

// Runs authentication and authorization before Wolverine maps protected HTTP endpoints.
app.UseAuthentication();
app.UseAuthorization();

// Maps standard Aspire endpoints such as health and liveness probes used by orchestration and diagnostics.
app.MapDefaultEndpoints();

// Maps Wolverine HTTP endpoints with FluentValidation problem details middleware.
app.MapModuleEndpoints();

foreach (var module in modules)
    await module.InitializeMigrationsAsync(app.Services);

// Starts the web application and begins accepting requests.
app.Run();
