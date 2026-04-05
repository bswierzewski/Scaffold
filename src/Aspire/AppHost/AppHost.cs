var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("compose");

var api = builder.AddProject<Projects.Scaffold_Api>("api");

// Add Frontend project (Vite + React)
var frontend = builder.AddViteApp("frontend", "../../Frontend/app");

// Add Caddy as a Reverse Proxy
builder.AddContainer("proxy", "caddy", "2.11-alpine")
    // Mount the Caddyfile configuration
    .WithBindMount("../../../infra/caddy/Caddyfile", "/etc/caddy/Caddyfile", isReadOnly: true)
    // Mount a persistent volume for certificates to avoid regeneration on restart
    .WithVolume("caddy_data", "/data")
    .WithHttpEndpoint(port: 80, targetPort: 80, name: "http")
    .WithHttpsEndpoint(port: 443, targetPort: 443, name: "https")
    // Pass internal Aspire service addresses to Caddy
    .WithEnvironment("API_UPSTREAM", api.GetEndpoint("http"))
    .WithEnvironment("FRONTEND_UPSTREAM", frontend.GetEndpoint("http"))
    // Base domain
    .WithEnvironment("DOMAIN", "localhost");

builder.Build().Run();
