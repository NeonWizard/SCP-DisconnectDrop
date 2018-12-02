# SCP-DisconnectDrop
A simple plugin for SCP:SL Smod2 servers, which drops a player's inventory in the event that they are disconnected. An example of when this is useful is when the only person with keycards in 914 times out. Without this plugin, all other players in 914 would be trapped indefinitely. It operates by lightweight caching minimal player information every so often, of which the interval is configurable.

## Plugin Installation
To install:
1. Grab newest version of DisconnectDrop: [Latest Release](https://github.com/NeonWizard/SCP-DisconnectDrop/releases/latest)
2. Navigate to your SCP Secret Lab folder.
3. Drag DisconnectDrop.dll into the sm_plugins folder.

## Servermod (This is required for any plugin)
ServerMod is a server side plugin system with a bunch of additional configuration options, bug fixes, security patches and some optimisations built in.
 * SMOD can be found here: [Smod Github](https://github.com/Grover-c13/Smod2)
 * SMOD Discord: https://discord.gg/8nvmMTr
 
## Configuration
Config Option | Value Type | Default Value | Description
--- | :---: | :---: | ---
ddrop_inventory_refreshrate | Int | 2 | How often player inventories are cached (in seconds).

*Note that all configs should go in your server config file, not config_remoteadmin.txt

### Place any suggestions/problems in [issues](https://github.com/NeonWizard/SCP-DisconnectDrop/issues)!
