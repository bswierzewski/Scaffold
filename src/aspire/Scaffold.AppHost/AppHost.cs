using Scaffold.AppHost;

var builder = DistributedApplication.CreateBuilder(args);
var databaseOnly = builder.Configuration.GetValue("Tests:DatabaseOnly", false);

// Add PostgreSQL database container
var postgres = builder.AddPostgres(ResourceNames.Postgres)
    .WithImage("postgres", "18-alpine")
    //.WithDataVolume() // Persist data between restarts
    .WithPgWeb(); // GUI for managing the database

// Add database
var database = postgres.AddDatabase(ResourceNames.Database);

if (!databaseOnly)
{
    // Add API project
    var api = builder.AddProject<Projects.Scaffold_Api>(ResourceNames.Api)
        .WithReference(database, "Default")
        .WaitFor(database)
        .WithHttpHealthCheck("/health");

    // Add Frontend project (Vite + React)
    var app = builder.AddViteApp(ResourceNames.App, "../../frontend")
        .WithHttpHealthCheck("/");

    builder.AddYarp(ResourceNames.Gateway)
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
}

builder.Build().Run();
