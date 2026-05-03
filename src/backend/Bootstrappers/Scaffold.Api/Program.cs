using BuildingBlocks.Core.Interfaces;
using BuildingBlocks.Hosting;
using BuildingBlocks.Hosting.Enums;
using BuildingBlocks.Hosting.Extensions;
using BuildingBlocks.Infrastructure.Exceptions.Extensions;
using BuildingBlocks.Infrastructure.Exceptions.Handlers;
using BuildingBlocks.Infrastructure.Identity;
using BuildingBlocks.Infrastructure.OpenApi;
using BuildingBlocks.Infrastructure.Serilog.Extensions;
using BuildingBlocks.Infrastructure.Wolverine.Extensions;
using Scaffold.Api;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var executionMode = builder.GetExecutionMode();

// Adds Aspire defaults such as service discovery, health checks, telemetry wiring and resilient defaults
// shared across distributed application services.
if (executionMode != ApplicationExecutionMode.OpenApi)
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
    options.AddBearerSecurityScheme();
});

// Add user identity services and authentication/authorization middleware with JWT bearer support.
builder.Services.AddIdentity(builder.Configuration);

builder.Services.ConfigureModules(builder.Configuration, out IModule[] modules);

builder.ConfigureWolverine(executionMode, modules);

// Builds the DI container and the HTTP pipeline. After this point service registrations are closed.
var app = builder.Build();

await app.InitializeModulesAsync(modules, executionMode);

if (app.Environment.IsDevelopment())
{
    // Exposes the generated OpenAPI document only in development.
    app.MapOpenApi();

    // Scalar UI with JWT authorize support: http://localhost:7000/scalar/v1
    app.MapScalarApiReference();
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

await app.RunMigrationsAsync(modules, executionMode);

// Starts the web application and begins accepting requests.
app.Run();
