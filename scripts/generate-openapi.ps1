<#
.SYNOPSIS
    Generates the OpenAPI document for Scaffold.Api without triggering runtime side effects.

.DESCRIPTION
    Uses Microsoft.Extensions.ApiDescription.Server (dotnet build-time generation) with the
    --tooling argument forwarded to the application via OpenApiGeneratorCommandLineArgs.

    The --tooling argument activates tooling mode in Program.cs, which skips:
      - NpgsqlDataSource creation (no database connection)
      - Wolverine transport persistence
      - Module initialization (no migrations or seeding)

    The generated document is written to the path configured by OpenApiDocumentsDirectory
    in Scaffold.Api.csproj (repo root /openapi/).

.NOTES
    Tooling mode can also be activated without this script by setting the environment variable:
      ASPNETCORE_ENVIRONMENT=Tooling
    before running dotnet build with OpenApiGenerateDocuments=true.
#>

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Green  = 'Green'
$Red    = 'Red'
$Cyan   = 'Cyan'

$RepoRoot  = Resolve-Path (Join-Path $PSScriptRoot '..')
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

# OpenApiGenerateDocuments=true   — enables the build-time document generation target that is
#                                   disabled by default in the csproj (avoids generation on
#                                   every normal build).
# OpenApiGeneratorCommandLineArgs — passes --tooling as a command-line argument to the app
#                                   when Microsoft.Extensions.ApiDescription.Server launches it,
#                                   activating tooling mode in Program.cs without relying on
#                                   an environment variable.
$output = & dotnet build $ApiProject `
    --configuration Release `
    --property:OpenApiGenerateDocuments=true `
    "--property:OpenApiGeneratorCommandLineArgs=--tooling" `
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
