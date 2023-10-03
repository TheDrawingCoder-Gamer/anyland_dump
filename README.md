# Anyland Exporter

Exports your inventory when you open it in anyland.

Only dumps when there is no `BepInEx/invdump` directory

## Installation

1. Follow install instructions for [BepInEx](https://docs.bepinex.dev/articles/user_guide/installation/index.html?tabs=tabid-win) 
(If you have flarn's mods installed you'll likely have to uninstall them while you have BepInEx installed)
2. Download [latest release](https://github.com/TheDrawingCoder-Gamer/anyland_dump/releases/latest)
3. Add plugin to `BepInEx/plugins`

## Building

To build you have to edit the path to your game in `Directory.Build.props`. Change this to your root anyland path, then build using 
your IDE or `dotnet build`. 


