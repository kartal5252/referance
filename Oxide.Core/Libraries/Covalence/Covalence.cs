using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Oxide.Core.Logging;
using Oxide.Core.Plugins;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x02000044 RID: 68
	public class Covalence : Library
	{
		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000289 RID: 649 RVA: 0x0000C763 File Offset: 0x0000A963
		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600028A RID: 650 RVA: 0x0000C766 File Offset: 0x0000A966
		// (set) Token: 0x0600028B RID: 651 RVA: 0x0000C76E File Offset: 0x0000A96E
		[LibraryProperty("Server")]
		public IServer Server { get; private set; }

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600028C RID: 652 RVA: 0x0000C777 File Offset: 0x0000A977
		// (set) Token: 0x0600028D RID: 653 RVA: 0x0000C77F File Offset: 0x0000A97F
		[LibraryProperty("Players")]
		public IPlayerManager Players { get; private set; }

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0000C788 File Offset: 0x0000A988
		[LibraryProperty("Game")]
		public string Game
		{
			get
			{
				ICovalenceProvider covalenceProvider = this.provider;
				return ((covalenceProvider != null) ? covalenceProvider.GameName : null) ?? string.Empty;
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0000C7A5 File Offset: 0x0000A9A5
		[LibraryProperty("ClientAppId")]
		public uint ClientAppId
		{
			get
			{
				ICovalenceProvider covalenceProvider = this.provider;
				if (covalenceProvider == null)
				{
					return 0U;
				}
				return covalenceProvider.ClientAppId;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x06000290 RID: 656 RVA: 0x0000C7B8 File Offset: 0x0000A9B8
		[LibraryProperty("ServerAppId")]
		public uint ServerAppId
		{
			get
			{
				ICovalenceProvider covalenceProvider = this.provider;
				if (covalenceProvider == null)
				{
					return 0U;
				}
				return covalenceProvider.ServerAppId;
			}
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000C7CB File Offset: 0x0000A9CB
		public string FormatText(string text)
		{
			return this.provider.FormatText(text);
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0000C7D9 File Offset: 0x0000A9D9
		public Covalence()
		{
			this.logger = Interface.Oxide.RootLogger;
		}

		// Token: 0x06000293 RID: 659 RVA: 0x0000C7F4 File Offset: 0x0000A9F4
		internal void Initialize()
		{
			Type baseType = typeof(ICovalenceProvider);
			IEnumerable<Type> enumerable = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type[] array = null;
				try
				{
					array = assembly.GetTypes();
				}
				catch (ReflectionTypeLoadException ex)
				{
					array = ex.Types;
				}
				catch (TypeLoadException ex2)
				{
					this.logger.Write(LogType.Warning, "Covalence: Type {0} could not be loaded from assembly '{1}': {2}", new object[]
					{
						ex2.TypeName,
						assembly.FullName,
						ex2
					});
				}
				if (array != null)
				{
					enumerable = (((enumerable != null) ? enumerable.Concat(array) : null) ?? array);
				}
			}
			if (enumerable == null)
			{
				this.logger.Write(LogType.Warning, "Covalence not available yet for this game", new object[0]);
				return;
			}
			TypeFilter <>9__1;
			List<Type> list = new List<Type>(enumerable.Where(delegate(Type t)
			{
				if (t != null && t.IsClass && !t.IsAbstract)
				{
					TypeFilter filter;
					if ((filter = <>9__1) == null)
					{
						filter = (<>9__1 = ((Type m, object o) => m == baseType));
					}
					return t.FindInterfaces(filter, null).Length == 1;
				}
				return false;
			}));
			if (list.Count == 0)
			{
				this.logger.Write(LogType.Warning, "Covalence not available yet for this game", new object[0]);
				return;
			}
			Type type;
			if (list.Count > 1)
			{
				type = list[0];
				StringBuilder stringBuilder = new StringBuilder();
				for (int j = 1; j < list.Count; j++)
				{
					if (j > 1)
					{
						stringBuilder.Append(',');
					}
					stringBuilder.Append(list[j].FullName);
				}
				this.logger.Write(LogType.Warning, "Multiple Covalence providers found! Using {0}. (Also found {1})", new object[]
				{
					type,
					stringBuilder
				});
			}
			else
			{
				type = list[0];
			}
			try
			{
				this.provider = (ICovalenceProvider)Activator.CreateInstance(type);
			}
			catch (Exception ex3)
			{
				this.logger.Write(LogType.Warning, "Got exception when instantiating Covalence provider, Covalence will not be functional for this session.", new object[0]);
				this.logger.Write(LogType.Warning, "{0}", new object[]
				{
					ex3
				});
				return;
			}
			this.Server = this.provider.CreateServer();
			this.Players = this.provider.CreatePlayerManager();
			this.cmdSystem = this.provider.CreateCommandSystemProvider();
			this.logger.Write(LogType.Info, "Using Covalence provider for game '{0}'", new object[]
			{
				this.provider.GameName
			});
		}

		// Token: 0x06000294 RID: 660 RVA: 0x0000CA3C File Offset: 0x0000AC3C
		public void RegisterCommand(string command, Plugin plugin, CommandCallback callback)
		{
			if (this.cmdSystem == null)
			{
				return;
			}
			try
			{
				this.cmdSystem.RegisterCommand(command, plugin, callback);
			}
			catch (CommandAlreadyExistsException)
			{
				string text = ((plugin != null) ? plugin.Name : null) ?? "An unknown plugin";
				this.logger.Write(LogType.Error, "{0} tried to register command '{1}', this command already exists and cannot be overridden!", new object[]
				{
					text,
					command
				});
			}
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000CAAC File Offset: 0x0000ACAC
		public void UnregisterCommand(string command, Plugin plugin)
		{
			ICommandSystem commandSystem = this.cmdSystem;
			if (commandSystem == null)
			{
				return;
			}
			commandSystem.UnregisterCommand(command, plugin);
		}

		// Token: 0x04000108 RID: 264
		private ICommandSystem cmdSystem;

		// Token: 0x04000109 RID: 265
		private ICovalenceProvider provider;

		// Token: 0x0400010A RID: 266
		private readonly Logger logger;
	}
}
