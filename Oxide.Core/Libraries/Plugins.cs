using System;
using System.Linq;
using Oxide.Core.Plugins;

namespace Oxide.Core.Libraries
{
	// Token: 0x0200003D RID: 61
	public class Plugins : Library
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600025D RID: 605 RVA: 0x0000BD81 File Offset: 0x00009F81
		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000BD84 File Offset: 0x00009F84
		// (set) Token: 0x0600025F RID: 607 RVA: 0x0000BD8C File Offset: 0x00009F8C
		public PluginManager PluginManager { get; private set; }

		// Token: 0x06000260 RID: 608 RVA: 0x0000BD95 File Offset: 0x00009F95
		public Plugins(PluginManager pluginmanager)
		{
			this.PluginManager = pluginmanager;
		}

		// Token: 0x06000261 RID: 609 RVA: 0x0000BDA4 File Offset: 0x00009FA4
		[LibraryFunction("Exists")]
		public bool Exists(string name)
		{
			return this.PluginManager.GetPlugin(name) != null;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x0000BDB5 File Offset: 0x00009FB5
		[LibraryFunction("Find")]
		public Plugin Find(string name)
		{
			return this.PluginManager.GetPlugin(name);
		}

		// Token: 0x06000263 RID: 611 RVA: 0x0000BDC3 File Offset: 0x00009FC3
		[LibraryFunction("CallHook")]
		public object CallHook(string hookname, params object[] args)
		{
			return Interface.Call(hookname, args);
		}

		// Token: 0x06000264 RID: 612 RVA: 0x0000BDCC File Offset: 0x00009FCC
		[LibraryFunction("GetAll")]
		public Plugin[] GetAll()
		{
			return this.PluginManager.GetPlugins().ToArray<Plugin>();
		}
	}
}
