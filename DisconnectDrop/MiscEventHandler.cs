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
	struct CachedPlayer
	{
		public List<Item> inventory;
		public Vector position;

		public CachedPlayer(List<Item> inv, Vector pos)
		{
			this.inventory = inv;
			this.position = pos;
		}
	}

	class MiscEventHandler : IEventHandlerPlayerJoin, IEventHandlerDisconnect, IEventHandlerFixedUpdate, IEventHandlerWaitingForPlayers, IEventHandlerRoundEnd, IEventHandlerRoundStart
	{
		private readonly DisconnectDrop plugin;

		private readonly int refreshRate;

		private float pTime = 0;
		private bool roundOver = true;

		public Dictionary<string, CachedPlayer> cachedPlayers = new Dictionary<string, CachedPlayer>();

		public MiscEventHandler(DisconnectDrop plugin)
		{
			this.plugin = plugin;
			this.refreshRate = this.plugin.GetConfigInt("ddrop_inventory_refreshrate");
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			if (!this.plugin.GetConfigBool("ddrop_enable")) this.plugin.pluginManager.DisablePlugin(plugin);

			// refresh these on round restart
			this.cachedPlayers = new Dictionary<string, CachedPlayer>();
		}

		// this is crucial so inventories aren't mass-dropped on server restart
		public void OnRoundEnd(RoundEndEvent ev)
		{
			this.roundOver = true;
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			this.roundOver = false;
		}

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			this.cachedPlayers[ev.Player.SteamId] = new CachedPlayer(new List<Item>(), Vector.Zero);
		}

		public void OnDisconnect(DisconnectEvent ev)
		{
			if (this.roundOver) return;

			this.plugin.Debug("Dropping player inventory.");
			Thread myThread = new Thread(new ThreadStart(RealDisconnectHandler));
			myThread.Start();
		}

		private void RealDisconnectHandler()
		{
			// bro this is so stupid. equivalent of setting a 500 ms timeout to read a response from a web request
			// RIP functional OnDisconnect handler 2018-2018
			Thread.Sleep(500);

			if (plugin.Server == null) return;

			foreach (var cplaya in this.cachedPlayers)
			{
				bool hasfound = false;
				foreach (var player in plugin.Server.GetPlayers())
				{
					if (player.SteamId == cplaya.Key)
					{
						hasfound = true;
						break;
					}
				}
				if (!hasfound)
				{
					// Drop player's cached inventory
					foreach (var item in cplaya.Value.inventory)
					{
						var loc = cplaya.Value.position;

						plugin.Server.Map.SpawnItem(
							item.ItemType,
							loc,
							Vector.Zero
						);
					}

					// Remove player entry
					this.cachedPlayers.Remove(cplaya.Key);

					break;
				}
			}
		}

		public void OnFixedUpdate(FixedUpdateEvent ev)
		{
			if (this.roundOver || this.plugin.Server == null) return;

			// Update cached list of all player inventories every ddrop_inventory_refreshrate seconds
			pTime -= Time.fixedDeltaTime;
			if (pTime <= 0)
			{
				try
				{
					pTime = this.refreshRate;

					// Update cached information
					var players = plugin.Server
						.GetPlayers()
						//.Where(p => p.TeamRole.Role != Role.SPECTATOR && p.TeamRole.Role != Role.UNASSIGNED)
						.ToList();

					for (int i = 0; i < players.Count; i++)
					{
						var player = players[i];
						this.cachedPlayers[player.SteamId] = new CachedPlayer(player.GetInventory(), player.GetPosition());
					};
				}
				catch (Exception e)
				{
					plugin.Debug("ERROR CAUGHT, OUTPUTTING...");
					plugin.Error(e.StackTrace);
				}
			}
		}
	}
}
