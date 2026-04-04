# src/Aspire/AppHost

This directory is intended for the `Scaffold.AppHost` project.

It is expected to contain:
- `Scaffold.AppHost.csproj` - an `aspire-apphost` project.
- source files configuring PostgreSQL, Traefik, the API, and the Vite application.
- dependency definitions between services used in the development environment.

This is the central composition point that wires local infrastructure and applications together.