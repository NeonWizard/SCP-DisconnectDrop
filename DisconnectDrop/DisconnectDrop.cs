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
        version = "1.0",
        SmodMajor = 3,
        SmodMinor = 1,
        SmodRevision = 22
    )]
    class DisconnectDrop : Plugin
    {
        public override void OnDisable()
        {
        }

        public override void OnEnable()
        {
            this.Info("DisconnectDrop has loaded successfully.");
            //this.Info("Config value: " + this.GetConfigString("test"));
        }

        public override void Register()
        {
            // Register config settings
            this.AddConfig(new ConfigSetting("ddrop_inventory_refreshrate", 1, SettingType.NUMERIC, true, "How often player inventories are cached (in seconds)."));

            // Register events
            this.AddEventHandlers(new DisconnectEventHandler(this), Priority.Normal);
        }
    }
}
