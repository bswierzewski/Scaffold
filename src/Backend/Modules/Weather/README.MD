# src/Backend/Modules/Weather

This directory is intended for the `Scaffold.Modules.Weather` module.

It is expected to contain:
- `Scaffold.Modules.Weather.csproj` - a `classlib` project.
- endpoints related to the Weather domain.
- module business services.
- module models and contracts.
- an optional `DbContext` and data mapping configuration for this module.

The module should remain isolated and should not assume the existence of other business modules.