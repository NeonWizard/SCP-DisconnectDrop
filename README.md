# DisconnectDrop
A simple plugin for SCP:SL Smod2 servers, which drops a player's inventory in the event that they are disconnected. An example of when this is useful is when the only person with keycards in 914 times out. Without this plugin, all other players in 914 would be trapped indefinitely. It operates by lightweight caching minimal player information every so often, of which the interval is configurable.

## Installation
**[Smod2](https://github.com/Grover-c13/Smod2) must be installed for this to work.**

1. Grab the [latest release](https://github.com/NeonWizard/SCP-DisconnectDrop/releases/latest) of DisconnectDrop.
2. Place DisconnectDrop.dll in your server's `sm_plugins` folder.

## Commands
Command | Value Type | Description
--- | :---: | ---
DDROPDISABLE | | **Disables the DisconnectDrop plugin.** Server will need to restart to enable it again.

## Configuration
Config Option | Value Type | Default Value | Description
:---: | :---: | :---: | ---
ddrop_enable | Bool | True | Whether DisconnectDrop should be enabled on server start.
ddrop_config | Bool | False | Enables debugging output for DisconnectDrop.
ddrop_inventory_refreshrate | Int | 5 | How often player inventories are cached (in seconds).

*Note that all configs should go in your server config file, not config_remoteadmin.txt

### Place any suggestions/problems in [issues](https://github.com/NeonWizard/SCP-DisconnectDrop/issues)!
