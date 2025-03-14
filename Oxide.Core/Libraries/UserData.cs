using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Oxide.Core.Libraries
{
	// Token: 0x0200003A RID: 58
	[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
	public class UserData
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600021C RID: 540 RVA: 0x0000A9DC File Offset: 0x00008BDC
		// (set) Token: 0x0600021D RID: 541 RVA: 0x0000A9E4 File Offset: 0x00008BE4
		public string LastSeenNickname { get; set; } = "Unnamed";

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600021E RID: 542 RVA: 0x0000A9ED File Offset: 0x00008BED
		// (set) Token: 0x0600021F RID: 543 RVA: 0x0000A9F5 File Offset: 0x00008BF5
		public HashSet<string> Perms { get; set; } = new HashSet<string>();

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000220 RID: 544 RVA: 0x0000A9FE File Offset: 0x00008BFE
		// (set) Token: 0x06000221 RID: 545 RVA: 0x0000AA06 File Offset: 0x00008C06
		public HashSet<string> Groups { get; set; } = new HashSet<string>();
	}
}
