using System;

namespace Oxide.Core.Extensions
{
	// Token: 0x02000052 RID: 82
	public abstract class Extension
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000312 RID: 786
		public abstract string Name { get; }

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000313 RID: 787
		public abstract string Author { get; }

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000314 RID: 788
		public abstract VersionNumber Version { get; }

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000315 RID: 789 RVA: 0x0000D40A File Offset: 0x0000B60A
		// (set) Token: 0x06000316 RID: 790 RVA: 0x0000D412 File Offset: 0x0000B612
		public string Filename { get; set; }

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x06000317 RID: 791 RVA: 0x0000D41B File Offset: 0x0000B61B
		public virtual string Branch { get; } = "public";

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x06000318 RID: 792 RVA: 0x0000D423 File Offset: 0x0000B623
		public virtual bool IsCoreExtension { get; }

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000319 RID: 793 RVA: 0x0000D42B File Offset: 0x0000B62B
		public virtual bool IsGameExtension { get; }

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600031A RID: 794 RVA: 0x0000D433 File Offset: 0x0000B633
		public virtual bool SupportsReloading { get; }

		// Token: 0x17000085 RID: 133
		// (get) Token: 0x0600031B RID: 795 RVA: 0x0000D43B File Offset: 0x0000B63B
		public ExtensionManager Manager { get; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x0600031C RID: 796 RVA: 0x0000D443 File Offset: 0x0000B643
		// (set) Token: 0x0600031D RID: 797 RVA: 0x0000D44B File Offset: 0x0000B64B
		public virtual string[] DefaultReferences { get; protected set; } = new string[0];

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600031E RID: 798 RVA: 0x0000D454 File Offset: 0x0000B654
		// (set) Token: 0x0600031F RID: 799 RVA: 0x0000D45C File Offset: 0x0000B65C
		public virtual string[] WhitelistAssemblies { get; protected set; } = new string[0];

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x06000320 RID: 800 RVA: 0x0000D465 File Offset: 0x0000B665
		// (set) Token: 0x06000321 RID: 801 RVA: 0x0000D46D File Offset: 0x0000B66D
		public virtual string[] WhitelistNamespaces { get; protected set; } = new string[0];

		// Token: 0x06000322 RID: 802 RVA: 0x0000D476 File Offset: 0x0000B676
		public Extension(ExtensionManager manager)
		{
			this.Manager = manager;
		}

		// Token: 0x06000323 RID: 803 RVA: 0x0000D4B4 File Offset: 0x0000B6B4
		public virtual void Load()
		{
		}

		// Token: 0x06000324 RID: 804 RVA: 0x0000D4B6 File Offset: 0x0000B6B6
		public virtual void Unload()
		{
		}

		// Token: 0x06000325 RID: 805 RVA: 0x0000D4B8 File Offset: 0x0000B6B8
		public virtual void LoadPluginWatchers(string pluginDirectory)
		{
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0000D4BA File Offset: 0x0000B6BA
		public virtual void OnModLoad()
		{
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0000D4BC File Offset: 0x0000B6BC
		public virtual void OnShutdown()
		{
		}
	}
}
