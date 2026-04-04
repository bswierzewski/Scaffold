# Scaffold

This directory is the root of the `Scaffold` solution.

The following files are intended to live here eventually:
- `Scaffold.slnx` - the solution file grouping all C# projects.
- `Directory.Build.props` - shared MSBuild settings for the entire repository.
- `Directory.Packages.props` - central NuGet package version management.
- `.gitignore` - ignore rules for `bin`, `obj`, `node_modules`, local certificates, and other temporary artifacts.

Subdirectories:
- `infra` - development infrastructure and reverse proxy configuration.
- `scripts` - helper scripts for migrations, API generation, and certificates.
- `src` - application source code.
- `tests` - test projects.