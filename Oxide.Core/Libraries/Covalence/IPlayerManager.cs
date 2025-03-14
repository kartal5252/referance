using System;
using System.Collections.Generic;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x0200004F RID: 79
	public interface IPlayerManager
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060002ED RID: 749
		IEnumerable<IPlayer> All { get; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060002EE RID: 750
		IEnumerable<IPlayer> Connected { get; }

		// Token: 0x060002EF RID: 751
		IPlayer FindPlayerById(string id);

		// Token: 0x060002F0 RID: 752
		IPlayer FindPlayerByObj(object obj);

		// Token: 0x060002F1 RID: 753
		IPlayer FindPlayer(string partialNameOrId);

		// Token: 0x060002F2 RID: 754
		IEnumerable<IPlayer> FindPlayers(string partialNameOrId);
	}
}
