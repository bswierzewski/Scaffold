# Backend Modules

This folder contains example business modules that show the expected module shape in this solution.

Each module demonstrates:
- a module bootstrap class implementing `IModule`
- PostgreSQL-backed EF Core persistence with a dedicated schema
- Wolverine HTTP endpoints with OpenAPI metadata via `Tags`, `EndpointName`, and `EndpointSummary`
- a small module-specific service such as `IPrinter`
- integration and end-to-end test examples that exercise `POST` and `GET` handlers

The sample modules added here are:
- `Scaffold.Modules.Catalog`
- `Scaffold.Modules.Notifications`

Look at the API bootstrapper and tests for the end-to-end wiring:
- `src/backend/Bootstrappers/Scaffold.Api/Program.cs`
- `tests/Scaffold.Tests.Integration`
- `tests/Scaffold.Tests.E2E`