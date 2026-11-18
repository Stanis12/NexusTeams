param([switch]$SkipBuild)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root 'src/NexusTeams.Plugin/NexusTeams.Plugin.csproj'
$rolesProject = Join-Path $root '..\NuageREPO-main\NuageREPO-main\mods\NuageRoles\src\NexusRoles.Plugin\NexusRoles.Plugin.csproj'
$rolesDll = Join-Path (Split-Path -Parent $rolesProject) 'bin/Release/netstandard2.1/NexusRoles.Plugin.dll'
if (-not (Test-Path $rolesDll)) { throw "Build NexusRoles first; expected API DLL at $rolesDll" }
$env:NexusRolesPluginPath = $rolesDll
$buildOutput = Join-Path $root 'out/'
if (-not $SkipBuild) { dotnet build $project -c Release "-p:BaseOutputPath=$buildOutput" -p:UseSharedCompilation=false; if ($LASTEXITCODE -ne 0) { throw 'NexusTeams build failed.' } }
$out = Join-Path $root 'release/staging'
New-Item -ItemType Directory -Force -Path (Join-Path $out 'plugins') | Out-Null
Copy-Item (Join-Path $root 'out/Release/netstandard2.1/NexusTeams.Plugin.dll') (Join-Path $out 'plugins/NexusTeams.Plugin.dll') -Force
Copy-Item (Join-Path $root 'manifest.json') $out -Force
Copy-Item (Join-Path $root 'README.md') $out -Force
$zip = Join-Path $root 'release/NexusTeams-1.0.0.zip'
New-Item -ItemType Directory -Force -Path (Split-Path -Parent $zip) | Out-Null
Compress-Archive -Path (Join-Path $out '*') -DestinationPath $zip -Force
Remove-Item $out -Recurse -Force
Write-Host "Created $zip"
