using Smod2;
using Smod2.Attributes;
using Smod2.EventHandlers;
using Smod2.Events;
using System.Collections.Generic;

namespace DisconnectDrop
{
    [PluginDetails(
        author = "Spooky",
        name = "DeathDrop",
        description = "Drops player items on disconnection.",
        id = "xyz.wizardlywonders.DeathDrop",
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
            //this.AddConfig(new Smod2.Config.ConfigSetting("test", "yes", Smod2.Config.SettingType.STRING, true, "test"));

            // Register events
            this.AddEventHandler(typeof(IEventHandlerDisconnect), new DisconnectEventHandler(this), Priority.Highest);
        }
    }
}
