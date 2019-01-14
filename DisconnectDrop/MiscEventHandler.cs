using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

namespace DisconnectDrop
{
	class MiscEventHandler : IEventHandlerPlayerJoin, IEventHandlerDisconnect, IEventHandlerRoundRestart, IEventHandlerFixedUpdate, IEventHandlerWaitingForPlayers
	{
		private readonly DisconnectDrop plugin;

		private float pTime = 0;
		public Dictionary<string, List<Item>> inventories = new Dictionary<string, List<Item>>(); // steamId: inventory
		public Dictionary<string, Vector> locations = new Dictionary<string, Vector>();           // steamId: position

		public MiscEventHandler(DisconnectDrop plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!this.plugin.GetConfigBool("ddrop_enable")) this.plugin.pluginManager.DisablePlugin(plugin);
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			inventories[ev.Player.SteamId] = new List<Item>();
			locations[ev.Player.SteamId] = Vector.Zero;
		}

		public void OnDisconnect(DisconnectEvent ev)
		{
			Thread myThread = new Thread(new ThreadStart(RealDisconnectHandler));
			myThread.Start();
		}

		private void RealDisconnectHandler()
		{
			// bro this is so stupid. equivalent of setting a 500 ms timeout to read a response from a web request
			// RIP functional OnDisconnect handler 2018-2018
			Thread.Sleep(500);

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
							loc,
							Vector.Zero
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
			this.inventories = new Dictionary<string, List<Item>>();
			this.locations = new Dictionary<string, Vector>();
		}

		private int refreshRate = 2;
		private DateTime refreshCheck = DateTime.Now.AddSeconds(-1);

		internal int GetInvRefreshRate()
		{
			if (refreshCheck < DateTime.Now)
			{
				refreshRate = plugin.GetConfigInt("ddrop_inventory_refreshrate");
				refreshCheck = DateTime.Now.AddSeconds(20);
			}
			return refreshRate;
		}


		public void OnFixedUpdate(FixedUpdateEvent ev)
		{
			// Update cached list of all player inventories every ddrop_inventory_refreshrate seconds
			pTime -= Time.fixedDeltaTime;
			if (pTime < 0)
			{
				pTime = GetInvRefreshRate();

				try
				{
					// Update cached information
					var players = plugin.Server
						.GetPlayers()
						.Where(p => p.TeamRole.Role != Role.SPECTATOR && p.TeamRole.Role != Role.UNASSIGNED)
						.ToList();

					for (int i = 0; i < players.Count; i++)
					{
						var player = players[i];

						inventories[player.SteamId] = player.GetInventory();
						locations[player.SteamId] = player.GetPosition();
					};
				}
				catch (Exception e)
				{
					plugin.Error(e.StackTrace);
				}
			}
		}
	}
}
