param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "src/Authoring.LanguageServer.Catalog/Authoring.LanguageServer.Catalog.csproj"
$outDir = Join-Path $root "extensions/vscode-catalog/server"

dotnet publish $project -c $Configuration -o $outDir --nologo
Write-Host "Published catalog LSP to $outDir"
