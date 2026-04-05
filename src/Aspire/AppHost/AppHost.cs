var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Scaffold_Api>("api");

var frontend = builder.AddContainer("frontend", "nginx", "alpine")
    .WithBindMount("../../../src/Frontend/app", "/usr/share/nginx/html", isReadOnly: true)
    .WithHttpEndpoint(targetPort: 80, name: "http");

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
