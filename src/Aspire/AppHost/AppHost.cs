var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "17-alpine")
    //.WithDataVolume() // Persist data between restarts
    .WithPgWeb(); // GUI for managing the database

var db = postgres
    .AddDatabase("scaffold");

// Add API project
var api = builder.AddProject<Projects.Scaffold_Api>("api")
    .WaitFor(db)
    .WithReference(db, connectionName: "Default");

// Add Frontend project (Vite + React)
var app = builder.AddViteApp("app", "../../Frontend")
    .WithReference(api);

// Add Caddy reverse proxy
builder.AddContainer("proxy", "caddy", "2.11-alpine")
    .WithBindMount("../../../infra/caddy/Caddyfile", "/etc/caddy/Caddyfile", isReadOnly: true)
    .WithVolume("caddy_data", "/data")
    .WithVolume("caddy_config", "/config")
    .WithHttpEndpoint(port: 80, targetPort: 80, name: "http")
    .WithHttpsEndpoint(port: 443, targetPort: 443, name: "https")
    .WithEnvironment("API_UPSTREAM", api.GetEndpoint("http"))
    .WithEnvironment("APP_UPSTREAM", app.GetEndpoint("http"))
    .WithEnvironment("DOMAIN", "localhost")
    .WaitFor(api)
    .WaitFor(app);

// Docker Compose publish target (without Aspire dashboard)
builder.AddDockerComposeEnvironment("compose")
    .WithProperties(env => env.DashboardEnabled = false);

builder.Build().Run();
