using BuildingBlocks.Core.Abstractions;
using BuildingBlocks.Hosting;
using BuildingBlocks.Infrastructure.Exceptions.Handlers;
using BuildingBlocks.Infrastructure.Extensions;
using BuildingBlocks.Infrastructure.Middleware;
using BuildingBlocks.Infrastructure.Persistence.Extensions;
using BuildingBlocks.Infrastructure.Serilog.Extensions;
using Scaffold.Api.Authentication;
using Scaffold.Weather;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;
using Wolverine.Http;
using Wolverine.Http.FluentValidation;
using Wolverine.Postgresql;

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

// Creates one shared NpgsqlDataSource from configuration so modules and Wolverine can reuse
// the same PostgreSQL connection infrastructure instead of building their own separately.
var dataSource = builder.Services.AddNpgsqlDataSource(builder.Configuration);

// Lists application modules explicitly so the bootstrapper can register their services
// and expose their Wolverine handlers/endpoints.
IModule[] modules = [
    new WeatherModule()
];

// Lets each module register its own dependencies into the application's DI container.
foreach (var module in modules)
    module.AddServices(builder.Services, builder.Configuration);

// Configures Wolverine as the messaging and HTTP endpoint runtime for handlers discovered in modules.
builder.Host.UseWolverine(opts =>
{
    // Enables FluentValidation integration for Wolverine commands, messages and HTTP handlers.
    opts.UseFluentValidation();

    // Prevents Wolverine from chaining multiple handlers for the same message into one pipeline automatically.
    // Each matching handler is treated as a separate execution path.
    opts.MultipleHandlerBehavior = MultipleHandlerBehavior.Separated;

    // Persists Wolverine inbox/outbox and durable messaging state in PostgreSQL under the "wolverine" schema.
    opts.PersistMessagesWithPostgresql(dataSource, "wolverine");

    // Shares EF Core transactions with Wolverine so message dispatch and database changes commit atomically.
    opts.UseEntityFrameworkCoreTransactions();

    // Automatically wraps transactional handlers in the required transaction policy.
    opts.Policies.AutoApplyTransactions();

    // Includes shared BuildingBlocks Wolverine middleware such as payload logging.
    // Without this, discovery would only scan feature assemblies from application modules.
    opts.Discovery.IncludeAssembly(typeof(LoggingMiddleware).Assembly);

    // Includes each module assembly so Wolverine can discover handlers, endpoints and policies defined there.
    foreach (var module in modules)
        opts.Discovery.IncludeAssembly(module.GetType().Assembly);
});

// Adds the ASP.NET Core bridge for Wolverine HTTP endpoints so handlers annotated with Wolverine HTTP attributes
// can be exposed through the application's routing pipeline.
builder.Services.AddWolverineHttp();

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

// Maps Wolverine HTTP handlers into ASP.NET Core routing and adds FluentValidation-specific problem details
// middleware so validation failures are returned in a consistent API format.
app.MapWolverineEndpoints(opts =>
{
    opts.UseFluentValidationProblemDetailMiddleware();
});

// Runs post-build initialization for every module, which is the right place for startup tasks
// that need the final IServiceProvider.
foreach (var module in modules)
    await module.InitializeAsync(app.Services);

// Starts the web application and begins accepting requests.
app.Run();

// Make Program class accessible to integration tests
public partial class Program { }