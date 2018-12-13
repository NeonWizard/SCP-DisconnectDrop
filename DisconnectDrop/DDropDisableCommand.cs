using Smod2.Commands;
using Smod2;
using Smod2.API;
using System.IO;

namespace DisconnectDrop
{
	class DDropDisableCommand : ICommandHandler
	{
		private DisconnectDrop plugin;

		public DDropDisableCommand(DisconnectDrop plugin)
		{
			this.plugin = plugin;
		}

		public string GetCommandDescription()
		{
			return "Disables DisconnectDrop";
		}

		public string GetUsage()
		{
			return "DDROPDISABLE";
		}

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			plugin.Info(sender + " ran the " + GetUsage() + " command!");
			this.plugin.pluginManager.DisablePlugin(this.plugin);
			return new string[] { "DisconnectDrop Disabled" };
		}
	}
}
