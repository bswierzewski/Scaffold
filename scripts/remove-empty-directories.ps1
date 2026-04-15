<#
.SYNOPSIS
    Removes directories whose subtree contains no files.

.DESCRIPTION
    Traverses the directory tree from the deepest folders upward so parent folders
    that contain only empty subdirectories are also removed in the same run.
#>

[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Position = 0)]
    [string]$Path = (Join-Path $PSScriptRoot '..')
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$targetPath = Resolve-Path -Path $Path

if (-not (Test-Path -Path $targetPath -PathType Container)) {
    throw "Directory not found: $Path"
}

$removedCount = 0

$directories = Get-ChildItem -Path $targetPath -Directory -Recurse -Force |
    Sort-Object {
        ($_.FullName.TrimEnd([System.IO.Path]::DirectorySeparatorChar) -split '[\\/]').Count
    } -Descending

foreach ($directory in $directories) {
    $hasChildren = @(Get-ChildItem -Path $directory.FullName -Force).Count -gt 0

    if ($hasChildren) {
        continue
    }

    if ($PSCmdlet.ShouldProcess($directory.FullName, 'Remove empty directory')) {
        Remove-Item -Path $directory.FullName -Force
        $removedCount++
    }
}

Write-Host ("Removed {0} empty director{1} under {2}" -f $removedCount, $(if ($removedCount -eq 1) { 'y' } else { 'ies' }), $targetPath)