# scripts

This directory is intended for development scripts used by the `Scaffold` solution.

It is expected to contain PowerShell scripts such as:
- `ef-migrate.ps1` - runs `dotnet ef database update`.
- `generate-api.ps1` - starts the API and generates the frontend client through Orval.
- `setup-certs.ps1` - generates local certificates by using `mkcert`.

Scripts in this directory should be runnable by the team locally without requiring machine-specific path changes.