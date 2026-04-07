var builder = DistributedApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add PostgreSQL database container
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "18-alpine")
    //.WithDataVolume() // Persist data between restarts
    .WithPgWeb(); // GUI for managing the database

// Add database
postgres.AddDatabase("scaffold");

// Add API project
var api = builder.AddProject<Projects.Scaffold_Api>("api")
    .WithEnvironmentSection(configuration, "Configuration:Api");

// Add Frontend project (Vite + React)
var app = builder.AddViteApp("app", "../../Frontend/app");

builder.AddYarp("gateway")
    .WithConfiguration(yarp =>
    {
        yarp.AddRoute("/api/{**catch-all}", api);
        yarp.AddRoute("/{**catch-all}", app);
    });

builder.Build().Run();
