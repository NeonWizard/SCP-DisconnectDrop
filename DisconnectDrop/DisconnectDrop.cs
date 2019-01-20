using Smod2;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.Config;

namespace DisconnectDrop
{
	[PluginDetails(
		author = "Spooky",
		name = "DisconnectDrop",
		description = "Drops player items on disconnection.",
		id = "xyz.wizardlywonders.DisconnectDrop",
		version = "1.6.0",
		SmodMajor = 3,
		SmodMinor = 2,
		SmodRevision = 2
	)]
	class DisconnectDrop : Plugin
	{
		public override void OnDisable()
		{
			this.Info("DisconnectDrop has been disabled.");
		}

		public override void OnEnable()
		{
			this.Info("DisconnectDrop has loaded successfully.");
		}

		public override void Register()
		{
			// Register config settings
			this.AddConfig(new ConfigSetting("ddrop_enable", true, SettingType.BOOL, true, "Whether DisconnectDrop should be enabled on server start."));
			this.AddConfig(new ConfigSetting("ddrop_inventory_refreshrate", 2, SettingType.NUMERIC, true, "How often player inventories are cached (in seconds)."));

			// Register events
			this.AddEventHandlers(new MiscEventHandler(this), Priority.Highest);

			// Register commands
			this.AddCommand("ddropdisable", new DDropDisableCommand(this));
		}
	}
}
