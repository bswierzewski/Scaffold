<#
.SYNOPSIS
    Generates the OpenAPI document for Scaffold.Api without triggering runtime side effects.

.DESCRIPTION
    Uses Microsoft.Extensions.ApiDescription.Server (dotnet build-time generation) with
    ASPNETCORE_ENVIRONMENT set to "Tooling" for the duration of the build.

    dotnet-getdocument.dll launches the application in-process to extract the OpenAPI spec.
    Because it inherits the current process environment, setting ASPNETCORE_ENVIRONMENT=Tooling
    causes WebApplication.CreateBuilder to report that environment, which activates tooling mode
    in Program.cs and skips all runtime side effects:
      - NpgsqlDataSource creation (no database connection or connection pool)
      - Wolverine transport persistence
      - Module initialization (no migrations or seeding)

    The generated document is written to the path configured by OpenApiDocumentsDirectory
    in Scaffold.Api.csproj (repo root /openapi/).

.NOTES
    The same tooling mode can be activated when running the app directly by either setting
    ASPNETCORE_ENVIRONMENT=Tooling or passing the --tooling command-line argument.
#>

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Green  = 'Green'
$Red    = 'Red'
$Cyan   = 'Cyan'

$RepoRoot   = Resolve-Path (Join-Path $PSScriptRoot '..')
$ApiProject = Join-Path $RepoRoot 'src\backend\Bootstrappers\Scaffold.Api\Scaffold.Api.csproj'
$OutputDir  = Join-Path $RepoRoot 'openapi'

Write-Host "`n========================================"
Write-Host '  Generating OpenAPI document' -ForegroundColor $Cyan
Write-Host "========================================`n"

if (-not (Test-Path $ApiProject)) {
    Write-Host "ERROR: project not found at $ApiProject" -ForegroundColor $Red
    exit 1
}

Write-Host "Project : $ApiProject"
Write-Host "Output  : $OutputDir"
Write-Host ''

# Set ASPNETCORE_ENVIRONMENT=Tooling for the duration of this script session.
# dotnet-getdocument.dll inherits this env var when it launches the application to extract
# the OpenAPI spec, so builder.Environment.IsEnvironment("Tooling") returns true and the
# runtime infrastructure (NpgsqlDataSource, Wolverine, module initialization) is skipped.
$env:ASPNETCORE_ENVIRONMENT = 'Tooling'

# OpenApiGenerateDocuments=true  — enables the build-time generation target; disabled by
#                                  default in the csproj so normal builds stay fast.
$output = & dotnet build $ApiProject `
    --configuration Release `
    --property:OpenApiGenerateDocuments=true `
    2>&1

$exitCode = $LASTEXITCODE
$outputText = $output -join "`n"

if ($exitCode -ne 0) {
    Write-Host 'FAILED' -ForegroundColor $Red
    Write-Host $outputText -ForegroundColor 'Gray'
    exit 1
}

$generatedFile = Join-Path $OutputDir 'Scaffold.Api.json'

if (-not (Test-Path $generatedFile)) {
    Write-Host "ERROR: build succeeded but output file not found: $generatedFile" -ForegroundColor $Red
    Write-Host 'Build output:' -ForegroundColor 'Gray'
    Write-Host $outputText -ForegroundColor 'Gray'
    exit 1
}

$fileSize = (Get-Item $generatedFile).Length
Write-Host "SUCCESS  ->  $generatedFile  ($fileSize bytes)" -ForegroundColor $Green
Write-Host ''
