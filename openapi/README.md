# OpenAPI

This folder stores generated OpenAPI documents shared across the repository.

- `scaffold-api.json` is generated from `src/Backend/Host/Api/Scaffold.Api.csproj` during `dotnet build`.
- Frontend client generation in `src/Frontend/app` reads this file through Orval.