using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventHandlers;

namespace DisconnectDrop
{
    class DisconnectEventHandler : IEventHandlerDisconnect
    {
        private Plugin plugin;

        public DisconnectEventHandler(Plugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnDisconnect(DisconnectEvent ev)
        {
            throw new System.NotImplementedException();
        }
    }
}
