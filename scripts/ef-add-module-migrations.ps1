<#
.SYNOPSIS
    Checks EF Core model changes and generates migrations for the configured modules.
#>

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$Green = 'Green'
$Red = 'Red'
$Yellow = 'DarkYellow'
$Cyan = 'Cyan'

$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
Set-Location $RepoRoot

$StartupProject = 'src/backend/Bootstrappers/Scaffold.Api/Scaffold.Api.csproj'
$MigrationsOutputDir = 'Infrastructure/Persistence/Migrations'

$Modules = @(
    [pscustomobject]@{
        Name = 'Weather'
        Context = 'WeatherDbContext'
        Project = 'src/backend/Modules/Weather/Scaffold.Weather/Scaffold.Weather.csproj'
    }
)

Write-Host 'Building startup project to ensure everything is up to date...' -ForegroundColor $Cyan

$buildProcess = Start-Process `
    -FilePath 'dotnet' `
    -ArgumentList @('build', $StartupProject, '-v', 'q') `
    -Wait `
    -NoNewWindow `
    -PassThru

if ($buildProcess.ExitCode -ne 0) {
    Write-Host 'FATAL: Build failed! Cannot proceed with migrations.' -ForegroundColor $Red
    exit 1
}

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

    $efBaseArgs = @(
        'ef', 'migrations', 'has-pending-model-changes',
        '--context', $module.Context,
        '--project', $module.Project,
        '--startup-project', $StartupProject,
        '--no-build'
    )

    $output = & dotnet @efBaseArgs 2>&1
    $exitCode = $LASTEXITCODE
    $outputText = $output -join "`n"

    if ($exitCode -eq 0) {
        Write-Host 'no changes' -ForegroundColor $Yellow
        $skipped++
        continue
    }

    if ($outputText -match 'No DbContext named') {
        Write-Host 'SKIPPED (DbContext not found)' -ForegroundColor $Yellow
        $skipped++
        continue
    }

    if ($outputText -match 'Unable to create|Build failed|Connection refused') {
        Write-Host 'ERROR (Infrastructure or Build failure)' -ForegroundColor $Red
        Write-Host $outputText -ForegroundColor 'Gray'
        $failed++
        continue
    }

    if ($outputText -notmatch 'pending model changes|changes have been made to the model') {
        Write-Host 'ERROR' -ForegroundColor $Red
        Write-Host $outputText -ForegroundColor 'Gray'
        $failed++
        continue
    }

    Write-Host 'CHANGES DETECTED! Creating migration...' -ForegroundColor $Cyan

    $timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'
    $migrationName = "Auto_$timestamp"

    $efAddArgs = @(
        'ef', 'migrations', 'add', $migrationName,
        '--context', $module.Context,
        '--project', $module.Project,
        '--startup-project', $StartupProject,
        '--no-build',
        '--output-dir', $MigrationsOutputDir
    )

    $addOutput = & dotnet @efAddArgs 2>&1
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