<#
.SYNOPSIS
    Checks EF Core model changes and generates migrations for the configured modules.

.NOTES
    Each module project must contain an IDesignTimeDbContextFactory implementation.
    This allows EF Tools to instantiate the DbContext directly without a startup project,
    avoiding side effects from the API bootstrapper (Wolverine, NpgsqlDataSource, etc.).
#>

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Green = 'Green'
$Red = 'Red'
$Yellow = 'DarkYellow'
$Cyan = 'Cyan'

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $RepoRoot

$MigrationsOutputDir = 'Infrastructure/Persistence/Migrations'

$Modules = @(
    [pscustomobject]@{
        Name = 'Announcements'
        Context = 'AnnouncementsDbContext'
        Project = 'src/backend/Modules/Announcements/Scaffold.Announcements/Scaffold.Announcements.csproj'
    }
    [pscustomobject]@{
        Name = 'Weather'
        Context = 'WeatherDbContext'
        Project = 'src/backend/Modules/Weather/Scaffold.Weather/Scaffold.Weather.csproj'
    }
)

Write-Host "`n========================================"
Write-Host '  Checking Migrations' -ForegroundColor $Cyan
Write-Host "========================================`n"

$created = 0
$skipped = 0
$failed = 0

foreach ($module in $Modules) {
    Write-Host (' [{0,-18}] : ' -f $module.Name) -NoNewline

    if (-not (Test-Path $module.Project)) {
        Write-Host 'SKIPPED (project file not found)' -ForegroundColor $Yellow
        $skipped++
        continue
    }

    $output = & dotnet ef migrations has-pending-model-changes `
        --context $module.Context `
        --project $module.Project `
        2>&1

    $exitCode = $LASTEXITCODE
    $outputText = $output -join "`n"

    if ($exitCode -eq 0) {
        Write-Host 'no changes' -ForegroundColor $Yellow
        $skipped++
        continue
    }

    if ($outputText -match 'No DbContext named|Unable to create|Build failed') {
        Write-Host 'ERROR' -ForegroundColor $Red
        Write-Host $outputText -ForegroundColor 'Gray'
        $failed++
        continue
    }

    Write-Host 'CHANGES DETECTED! Creating migration...' -ForegroundColor $Cyan

    $timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
    $migrationName = "Auto_$timestamp"

    $addOutput = & dotnet ef migrations add $migrationName `
        --context $module.Context `
        --project $module.Project `
        --output-dir $MigrationsOutputDir `
        2>&1

    $addExitCode = $LASTEXITCODE

    Write-Host (' [{0,-18}] : ' -f $module.Name) -NoNewline

    if ($addExitCode -eq 0) {
        Write-Host "SUCCESS ($migrationName)" -ForegroundColor $Green
        $created++
        continue
    }

    Write-Host 'ERROR (Failed to generate migration)' -ForegroundColor $Red
    Write-Host ($addOutput -join "`n") -ForegroundColor 'Gray'
    $failed++
}

Write-Host "`n========================================"
Write-Host "Created:  $created" -ForegroundColor $Green
Write-Host "Skipped:  $skipped" -ForegroundColor $Yellow
Write-Host "Failed:   $failed" -ForegroundColor $Red
Write-Host "========================================`n"

if ($failed -gt 0) {
    exit 1
}