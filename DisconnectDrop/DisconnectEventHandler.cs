using Smod2;
using Smod2.Events;
using Smod2.EventHandlers;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace DisconnectDrop
{
    class DisconnectEventHandler : IEventHandlerDisconnect, IEventHandlerFixedUpdate
    {
        private Plugin plugin;

        private float pTime;
        public Dictionary<string, List<Smod2.API.Item>> inventories;

        public DisconnectEventHandler(Plugin plugin)
        {
            this.plugin = plugin;

            this.pTime = 0;
            this.inventories = new Dictionary<string, List<Smod2.API.Item>>();
        }

        public void OnDisconnect(DisconnectEvent ev)
        {
            throw new System.NotImplementedException();
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
                        var obj = (GameObject)players[i].GetGameObject();

                        // '[i] = x' as opposed to .Add because we want to override old values
                        inventories[player.SteamId] = player.GetInventory();
                    }
                }
                catch (Exception e)
                {
                    plugin.Info(e.StackTrace);
                }
            }
        }
    }
}
