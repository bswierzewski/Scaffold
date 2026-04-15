using BuildingBlocks.Infrastructure.Configuration;

EnvLoader.Load(AppContext.BaseDirectory);

var builder = DistributedApplication.CreateBuilder(args);

const string clerkAuthenticationSectionPath = "Authentication:Clerk";

// Add PostgreSQL database container
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "18-alpine")
    //.WithDataVolume() // Persist data between restarts
    .WithPgWeb(); // GUI for managing the database

// Add database
var database = postgres.AddDatabase("scaffold");

// Run database migrations before starting the API.
var dbMigrator = builder.AddProject<Projects.Scaffold_DbMigrator>("db-migrator")
    .WithReference(database, "Default")
    .WaitFor(database);

// Add API project
var api = builder.AddProject<Projects.Scaffold_Api>("api")
    .WithReference(database, "Default")
    .WithEnvironmentSection(builder.Configuration, clerkAuthenticationSectionPath)
    .WaitFor(database)
    .WaitForCompletion(dbMigrator)
    .WithHttpHealthCheck("/health");

// Add Frontend project (Vite + React)
var app = builder.AddViteApp("app", "../../frontend")
    .WithHttpHealthCheck("/");

builder.AddYarp("gateway")
    .WithHttpsEndpoint()
    .WithHttpsDeveloperCertificate()
    .WithEndpoint("http", endpoint =>
    {
        endpoint.IsExternal = false;
        endpoint.ExcludeReferenceEndpoint = true;
    })
    .WaitFor(api)
    .WaitFor(app)
    .WithHttpHealthCheck("/")
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/api/{**catch-all}", api);
        yarp.AddRoute("/{**catch-all}", app);
    });

builder.Build().Run();
