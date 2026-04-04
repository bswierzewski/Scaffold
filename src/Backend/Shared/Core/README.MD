# src/Backend/Shared/Core

This directory is intended for the `Scaffold.Shared.Core` project.

It is expected to contain:
- `Scaffold.Shared.Core.csproj` - a `classlib` project.
- interfaces.
- base exceptions.
- foundational shared types, for example `Result<T>`.
- contracts used by modules and the host.

This library should avoid external infrastructure dependencies and remain as clean as possible.