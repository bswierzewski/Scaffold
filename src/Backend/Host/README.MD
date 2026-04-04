# src/Backend/Host

This directory is intended for the `Scaffold.Api` project.

It is expected to contain:
- `Scaffold.Api.csproj` - a `webapi` project.
- `Program.cs` - the application entry point.
- Swagger configuration.
- module registration.
- Minimal API endpoint definitions or integration with endpoints exposed by modules.

The host should act as the composition layer and should not contain module-specific business logic.