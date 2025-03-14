using System;
using System.IO;

namespace Oxide.Core.Plugins.Watchers
{
	// Token: 0x02000029 RID: 41
	public sealed class FileChange
	{
		// Token: 0x1700003E RID: 62
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00009645 File Offset: 0x00007845
		// (set) Token: 0x060001B8 RID: 440 RVA: 0x0000964D File Offset: 0x0000784D
		public string Name { get; private set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x00009656 File Offset: 0x00007856
		// (set) Token: 0x060001BA RID: 442 RVA: 0x0000965E File Offset: 0x0000785E
		public WatcherChangeTypes ChangeType { get; private set; }

		// Token: 0x060001BB RID: 443 RVA: 0x00009667 File Offset: 0x00007867
		public FileChange(string name, WatcherChangeTypes changeType)
		{
			this.Name = name;
			this.ChangeType = changeType;
		}
	}
}
