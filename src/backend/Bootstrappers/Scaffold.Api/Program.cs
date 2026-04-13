using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Hosting;
using BuildingBlocks.Infrastructure.Exceptions.Handlers;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Modules;
using BuildingBlocks.Infrastructure.Serilog.Extensions;
using Scaffold.Announcements;
using Scaffold.Api.Authentication;
using Scaffold.Weather;

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

// Provides a temporary current-user implementation so infrastructure depending on ICurrentUser
// can work before real authentication and claims mapping are introduced.
builder.Services.AddScoped<ICurrentUser, DummyUserContext>();

// Lists application modules explicitly so the bootstrapper can register their services
// and expose their Wolverine handlers/endpoints.
IModule[] modules = [
    new AnnouncementsModule(),
    new WeatherModule()
];

// Tooling mode disables all runtime side effects: no database connection, no Wolverine
// transport persistence, and no module initialization.
// Activated by setting ASPNETCORE_ENVIRONMENT=Tooling — e.g. via the generate-openapi script
// or a CI environment variable.
if (builder.Environment.IsEnvironment("Tooling"))
    builder.AddModularToolingInfrastructure(modules);
else
    builder.AddModularRuntimeInfrastructure(modules);

// Builds the DI container and the HTTP pipeline. After this point service registrations are closed.
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Exposes the generated OpenAPI document only in development.
    app.MapOpenApi();
}

// Enables the global exception handler middleware registered earlier.
app.UseExceptionHandler();

// Maps standard Aspire endpoints such as health and liveness probes used by orchestration and diagnostics.
app.MapDefaultEndpoints();

// Maps Wolverine HTTP endpoints with FluentValidation problem details middleware.
app.MapModularEndpoints();

// Starts the web application and begins accepting requests.
app.Run();

// Make Program class accessible to integration tests
public partial class Program { }