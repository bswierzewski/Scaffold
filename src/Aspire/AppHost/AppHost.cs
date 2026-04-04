var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Scaffold_Api>("scaffold-api");

builder.Build().Run();
