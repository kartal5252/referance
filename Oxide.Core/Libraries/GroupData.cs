using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Oxide.Core.Libraries
{
	// Token: 0x0200003B RID: 59
	[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
	public class GroupData
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000223 RID: 547 RVA: 0x0000AA38 File Offset: 0x00008C38
		// (set) Token: 0x06000224 RID: 548 RVA: 0x0000AA40 File Offset: 0x00008C40
		public string Title { get; set; } = string.Empty;

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000225 RID: 549 RVA: 0x0000AA49 File Offset: 0x00008C49
		// (set) Token: 0x06000226 RID: 550 RVA: 0x0000AA51 File Offset: 0x00008C51
		public int Rank { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000227 RID: 551 RVA: 0x0000AA5A File Offset: 0x00008C5A
		// (set) Token: 0x06000228 RID: 552 RVA: 0x0000AA62 File Offset: 0x00008C62
		public HashSet<string> Perms { get; set; } = new HashSet<string>();

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000229 RID: 553 RVA: 0x0000AA6B File Offset: 0x00008C6B
		// (set) Token: 0x0600022A RID: 554 RVA: 0x0000AA73 File Offset: 0x00008C73
		public string ParentGroup { get; set; } = string.Empty;
	}
}
