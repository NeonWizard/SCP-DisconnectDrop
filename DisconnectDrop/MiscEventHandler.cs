using Smod2;
using Smod2.Events;
using Smod2.EventHandlers;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace DisconnectDrop
{
    class MiscEventHandler : IEventHandlerPlayerJoin, IEventHandlerDisconnect, IEventHandlerRoundRestart, IEventHandlerFixedUpdate
    {
        private Plugin plugin;

        private float pTime;
        public Dictionary<string, List<Smod2.API.Item>> inventories; // steamId: inventory
        public Dictionary<string, List<float>> locations;            // steamId: x, y, z

        public MiscEventHandler(Plugin plugin)
        {
            this.plugin = plugin;

            this.pTime = 0;
            this.inventories = new Dictionary<string, List<Smod2.API.Item>>();
            this.locations = new Dictionary<string, List<float>>();
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            inventories[ev.Player.SteamId] = new List<Smod2.API.Item>();
            locations[ev.Player.SteamId] = new List<float>() { 0, 0, 0 };
        }

        public void OnDisconnect(DisconnectEvent ev)
        {
            System.Threading.Thread myThread = new System.Threading.Thread(new System.Threading.ThreadStart(RealDisconnectHandler));
            myThread.Start();
        }

        private void RealDisconnectHandler()
        {
            // bro this is so stupid. equivalent of setting a 500 ms timeout to read a response from a web request
            // RIP functional OnDisconnect handler 2018-2018
            System.Threading.Thread.Sleep(500);

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
                    // Drop player's cached inventory
                    foreach (var item in inv.Value)
                    {
                        var loc = locations[inv.Key];

                        plugin.Server.Map.SpawnItem(
                            item.ItemType,
                            new Smod2.API.Vector(loc[0], loc[1], loc[2]),
                            Smod2.API.Vector.Zero
                        );
                    }

                    // Remove player entries
                    inventories.Remove(inv.Key);
                    locations.Remove(inv.Key);
                    break;
                }
            }
        }

        public void OnRoundRestart(RoundRestartEvent ev)
        {
            this.inventories = new Dictionary<string, List<Smod2.API.Item>>();
            this.locations = new Dictionary<string, List<float>>();
        }

        public void OnFixedUpdate(FixedUpdateEvent ev)
        {
            // Update cached list of all player inventories every ddrop_inventory_refreshrate seconds
            pTime -= Time.fixedDeltaTime;
            if (pTime < 0)
            {
                pTime = ConfigManager.Manager.Config.GetIntValue("ddrop_inventory_refreshrate", 2);

                try
                {
                    // Update cached information
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
