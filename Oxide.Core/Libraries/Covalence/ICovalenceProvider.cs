using System;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x0200004C RID: 76
	public interface ICovalenceProvider
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060002B1 RID: 689
		string GameName { get; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060002B2 RID: 690
		uint ClientAppId { get; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060002B3 RID: 691
		uint ServerAppId { get; }

		// Token: 0x060002B4 RID: 692
		ICommandSystem CreateCommandSystemProvider();

		// Token: 0x060002B5 RID: 693
		IPlayerManager CreatePlayerManager();

		// Token: 0x060002B6 RID: 694
		IServer CreateServer();

		// Token: 0x060002B7 RID: 695
		string FormatText(string text);
	}
}
