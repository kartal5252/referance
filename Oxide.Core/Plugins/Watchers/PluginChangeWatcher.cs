using System;

namespace Oxide.Core.Plugins.Watchers
{
	// Token: 0x0200002E RID: 46
	public abstract class PluginChangeWatcher
	{
		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060001CE RID: 462 RVA: 0x00009958 File Offset: 0x00007B58
		// (remove) Token: 0x060001CF RID: 463 RVA: 0x00009990 File Offset: 0x00007B90
		public event PluginChangeEvent OnPluginSourceChanged;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x060001D0 RID: 464 RVA: 0x000099C8 File Offset: 0x00007BC8
		// (remove) Token: 0x060001D1 RID: 465 RVA: 0x00009A00 File Offset: 0x00007C00
		public event PluginAddEvent OnPluginAdded;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x060001D2 RID: 466 RVA: 0x00009A38 File Offset: 0x00007C38
		// (remove) Token: 0x060001D3 RID: 467 RVA: 0x00009A70 File Offset: 0x00007C70
		public event PluginRemoveEvent OnPluginRemoved;

		// Token: 0x060001D4 RID: 468 RVA: 0x00009AA5 File Offset: 0x00007CA5
		protected void FirePluginSourceChanged(string name)
		{
			PluginChangeEvent onPluginSourceChanged = this.OnPluginSourceChanged;
			if (onPluginSourceChanged == null)
			{
				return;
			}
			onPluginSourceChanged(name);
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00009AB8 File Offset: 0x00007CB8
		protected void FirePluginAdded(string name)
		{
			PluginAddEvent onPluginAdded = this.OnPluginAdded;
			if (onPluginAdded == null)
			{
				return;
			}
			onPluginAdded(name);
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00009ACB File Offset: 0x00007CCB
		protected void FirePluginRemoved(string name)
		{
			PluginRemoveEvent onPluginRemoved = this.OnPluginRemoved;
			if (onPluginRemoved == null)
			{
				return;
			}
			onPluginRemoved(name);
		}
	}
}
