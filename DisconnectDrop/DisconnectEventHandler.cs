using Smod2;
using Smod2.Events;
using Smod2.EventHandlers;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace DisconnectDrop
{
    class DisconnectEventHandler : IEventHandlerPlayerJoin, IEventHandlerDisconnect, IEventHandlerFixedUpdate
    {
        private Plugin plugin;

        private float pTime;
        public Dictionary<string, List<Smod2.API.Item>> inventories; // steamId: inventory
        public Dictionary<string, List<float>> locations;            // steamId: x, y, z

        public DisconnectEventHandler(Plugin plugin)
        {
            this.plugin = plugin;

            this.pTime = 0;
            this.inventories = new Dictionary<string, List<Smod2.API.Item>>();
            this.locations = new Dictionary<string, List<float>>();
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            plugin.Info("Player joined, creating cache entries.");
            inventories.Add(ev.Player.SteamId, new List<Smod2.API.Item>());
            locations.Add(ev.Player.SteamId, new List<float>() {0, 0, 0} );
        }

        public void OnDisconnect(DisconnectEvent ev)
        {
            plugin.Info("Player disconnected, searching for player...");
            foreach (var inv in inventories)
            {
                bool hasfound = false;
                foreach (var player in plugin.Server.GetPlayers())
                {
                    if (player.SteamId == inv.Key)
                    {
                        hasfound = true;
                        break;
                    }
                }
                if (!hasfound)
                {
                    plugin.Info("Player found.");
                    // Drop player's cached inventory
                    foreach (var item in inv.Value)
                    {
                        var loc = locations[inv.Key];
                        plugin.Info("Dropping at location " + loc[0] + ":" + loc[1] + ":" + loc[2]);
                        item.SetPosition(new Smod2.API.Vector( loc[0], loc[1], loc[2] ));
                        item.Drop();
                    }

                    // Remove player entries
                    inventories.Remove(inv.Key);
                    locations.Remove(inv.Key);
                    break;
                }
            }
        }

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            // Update cached list of all player inventories every ddrop_inventory_refreshrate seconds
            pTime -= Time.fixedDeltaTime;
            if (pTime < 0)
            {
                pTime = ConfigManager.Manager.Config.GetIntValue("ddrop_inventory_refreshrate", 1);

                try
                {
                    var players = plugin.Server.GetPlayers();
                    for (int i = 0; i < players.Count; i++)
                    {
                        var player = players[i];
                        var obj = (GameObject)player.GetGameObject();

                        inventories[player.SteamId] = player.GetInventory();
                        locations[player.SteamId] = new List<float>{ obj.transform.position.x, obj.transform.position.y, obj.transform.position.z };
                    };
                }
                catch (Exception e)
                {
                    plugin.Info(e.StackTrace);
                }
            }
        }
    }
}
