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
	class MiscEventHandler : IEventHandlerPlayerJoin, IEventHandlerDisconnect, IEventHandlerFixedUpdate, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd
	{
		private readonly DisconnectDrop plugin;

		private float pTime;
		public Dictionary<string, List<Item>> inventories; // steamId: inventory
		public Dictionary<string, Vector> locations;       // steamId: position
		bool roundOver;

		public MiscEventHandler(DisconnectDrop plugin) => this.plugin = plugin;

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!this.plugin.GetConfigBool("ddrop_enable")) this.plugin.pluginManager.DisablePlugin(plugin);

			this.inventories = new Dictionary<string, List<Item>>();
			this.locations = new Dictionary<string, Vector>();

			this.pTime = 0;
			this.roundOver = false;
		}

		// this is crucial so inventories aren't mass-dropped on server restart
		public void OnRoundEnd(RoundEndEvent ev)
		{
			this.roundOver = true;
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			inventories[ev.Player.SteamId] = new List<Item>();
			locations[ev.Player.SteamId] = Vector.Zero;
		}

		public void OnDisconnect(DisconnectEvent ev)
		{
			if (this.roundOver) return;

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
					plugin.Info(e.StackTrace);
				}
			}
		}
	}
}
