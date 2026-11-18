# NexusTeams

NexusTeams adds a compact teammate role panel to R.E.P.O. when used with NexusRoles. The panel sits at the right edge, only appears during a level, and can be hidden with **F8** (configurable). The toggle is ignored while chat is open.

## Settings

Edit `BepInEx/config/NexusTeams.cfg` in the **NexusTeams** section:

- `Toggle key`: panel visibility key (default `F8`)
- `Show at level start`: visibility when a level begins
- `Show local player`: include your own role
- `Panel scale`: size multiplier

Thunderstore publishing team: **nexi**.

## Build

Build requires the NexusRoles source checkout and .NET 9.0.100 or newer. Build NexusRoles first, then:

```powershell
$env:NexusRolesPluginPath = "..\NuageREPO-main\NuageREPO-main\mods\NuageRoles\src\NexusRoles.Plugin\bin\Release\netstandard2.1\NexusRoles.Plugin.dll"
dotnet restore src/NexusTeams.Plugin/NexusTeams.Plugin.csproj
dotnet build src/NexusTeams.Plugin/NexusTeams.Plugin.csproj -c Release
```

The NexusRoles DLL is a compile-time API reference and must not be included in the add-on package. All players in a lobby need NexusTeams installed for role updates to synchronize.

Source: [Stanis12/NexusTeams](https://github.com/Stanis12/NexusTeams)
