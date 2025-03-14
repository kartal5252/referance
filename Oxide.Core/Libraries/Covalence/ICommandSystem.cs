using System;
using Oxide.Core.Plugins;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x0200004B RID: 75
	public interface ICommandSystem
	{
		// Token: 0x060002AF RID: 687
		void RegisterCommand(string command, Plugin plugin, CommandCallback callback);

		// Token: 0x060002B0 RID: 688
		void UnregisterCommand(string command, Plugin plugin);
	}
}
