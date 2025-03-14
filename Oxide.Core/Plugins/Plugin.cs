using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Core.Plugins
{
	// Token: 0x02000025 RID: 37
	public abstract class Plugin
	{
		// Token: 0x0600015C RID: 348 RVA: 0x0000843A File Offset: 0x0000663A
		public static implicit operator bool(Plugin plugin)
		{
			return plugin != null;
		}

		// Token: 0x0600015D RID: 349 RVA: 0x00008440 File Offset: 0x00006640
		public static bool operator !(Plugin plugin)
		{
			return !plugin;
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600015E RID: 350 RVA: 0x0000844B File Offset: 0x0000664B
		// (set) Token: 0x0600015F RID: 351 RVA: 0x00008453 File Offset: 0x00006653
		public string Filename { get; protected set; }

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000160 RID: 352 RVA: 0x0000845C File Offset: 0x0000665C
		// (set) Token: 0x06000161 RID: 353 RVA: 0x00008464 File Offset: 0x00006664
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (string.IsNullOrEmpty(this.Name) || this.name == base.GetType().Name)
				{
					this.name = value;
				}
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00008492 File Offset: 0x00006692
		// (set) Token: 0x06000163 RID: 355 RVA: 0x0000849A File Offset: 0x0000669A
		public string Title { get; protected set; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000164 RID: 356 RVA: 0x000084A3 File Offset: 0x000066A3
		// (set) Token: 0x06000165 RID: 357 RVA: 0x000084AB File Offset: 0x000066AB
		public string Description { get; protected set; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000166 RID: 358 RVA: 0x000084B4 File Offset: 0x000066B4
		// (set) Token: 0x06000167 RID: 359 RVA: 0x000084BC File Offset: 0x000066BC
		public string Author { get; protected set; }

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000168 RID: 360 RVA: 0x000084C5 File Offset: 0x000066C5
		// (set) Token: 0x06000169 RID: 361 RVA: 0x000084CD File Offset: 0x000066CD
		public VersionNumber Version { get; protected set; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600016A RID: 362 RVA: 0x000084D6 File Offset: 0x000066D6
		// (set) Token: 0x0600016B RID: 363 RVA: 0x000084DE File Offset: 0x000066DE
		public int ResourceId { get; protected set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600016C RID: 364 RVA: 0x000084E7 File Offset: 0x000066E7
		// (set) Token: 0x0600016D RID: 365 RVA: 0x000084EF File Offset: 0x000066EF
		public PluginManager Manager { get; private set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600016E RID: 366 RVA: 0x000084F8 File Offset: 0x000066F8
		// (set) Token: 0x0600016F RID: 367 RVA: 0x00008500 File Offset: 0x00006700
		public bool HasConfig { get; protected set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000170 RID: 368 RVA: 0x00008509 File Offset: 0x00006709
		// (set) Token: 0x06000171 RID: 369 RVA: 0x00008511 File Offset: 0x00006711
		public bool HasMessages { get; protected set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000172 RID: 370 RVA: 0x0000851A File Offset: 0x0000671A
		// (set) Token: 0x06000173 RID: 371 RVA: 0x00008522 File Offset: 0x00006722
		public bool IsCorePlugin
		{
			get
			{
				return this.isCorePlugin;
			}
			set
			{
				if (!Interface.Oxide.HasLoadedCorePlugins)
				{
					this.isCorePlugin = value;
				}
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000174 RID: 372 RVA: 0x00008537 File Offset: 0x00006737
		// (set) Token: 0x06000175 RID: 373 RVA: 0x0000853F File Offset: 0x0000673F
		public PluginLoader Loader { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000176 RID: 374 RVA: 0x00008548 File Offset: 0x00006748
		public virtual object Object
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000854B File Offset: 0x0000674B
		// (set) Token: 0x06000178 RID: 376 RVA: 0x00008553 File Offset: 0x00006753
		public DynamicConfigFile Config { get; private set; }

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000179 RID: 377 RVA: 0x0000855C File Offset: 0x0000675C
		// (remove) Token: 0x0600017A RID: 378 RVA: 0x00008594 File Offset: 0x00006794
		public event PluginError OnError;

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600017B RID: 379 RVA: 0x000085C9 File Offset: 0x000067C9
		// (set) Token: 0x0600017C RID: 380 RVA: 0x000085D1 File Offset: 0x000067D1
		public bool IsLoaded { get; internal set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600017D RID: 381 RVA: 0x000085DA File Offset: 0x000067DA
		// (set) Token: 0x0600017E RID: 382 RVA: 0x000085E2 File Offset: 0x000067E2
		public double TotalHookTime { get; internal set; }

		// Token: 0x0600017F RID: 383 RVA: 0x000085EC File Offset: 0x000067EC
		protected Plugin()
		{
			this.Name = base.GetType().Name;
			this.Title = this.Name.Humanize();
			this.Author = "Unnamed";
			this.Version = new VersionNumber(1, 0, 0);
			this.commandInfos = new Dictionary<string, Plugin.CommandInfo>();
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00008682 File Offset: 0x00006882
		protected void Subscribe(string hook)
		{
			this.Manager.SubscribeToHook(hook, this);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00008691 File Offset: 0x00006891
		protected void Unsubscribe(string hook)
		{
			this.Manager.UnsubscribeToHook(hook, this);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x000086A0 File Offset: 0x000068A0
		public virtual void HandleAddedToManager(PluginManager manager)
		{
			this.Manager = manager;
			if (this.HasConfig)
			{
				this.LoadConfig();
			}
			if (this.HasMessages)
			{
				this.LoadDefaultMessages();
			}
			PluginManagerEvent onAddedToManager = this.OnAddedToManager;
			if (onAddedToManager != null)
			{
				onAddedToManager.Invoke(this, manager);
			}
			this.RegisterWithCovalence();
		}

		// Token: 0x06000183 RID: 387 RVA: 0x000086DE File Offset: 0x000068DE
		public virtual void HandleRemovedFromManager(PluginManager manager)
		{
			this.UnregisterWithCovalence();
			if (this.Manager == manager)
			{
				this.Manager = null;
			}
			PluginManagerEvent onRemovedFromManager = this.OnRemovedFromManager;
			if (onRemovedFromManager == null)
			{
				return;
			}
			onRemovedFromManager.Invoke(this, manager);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00008708 File Offset: 0x00006908
		public virtual void Load()
		{
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000870C File Offset: 0x0000690C
		public object CallHook(string hook, params object[] args)
		{
			float num = 0f;
			if (!this.IsCorePlugin && this.nestcount == 0)
			{
				this.preHookGcCount = GC.CollectionCount(0);
				num = Interface.Oxide.Now;
				this.stopwatch.Start();
				if (this.averageAt < 1f)
				{
					this.averageAt = num;
				}
			}
			this.TrackStart();
			this.nestcount++;
			object result;
			try
			{
				result = this.OnCallHook(hook, args);
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException(string.Format("Failed to call hook '{0}' on plugin '{1} v{2}'", hook, this.Name, this.Version), ex);
				result = null;
			}
			finally
			{
				this.nestcount--;
				this.TrackEnd();
				if (num > 0f)
				{
					this.stopwatch.Stop();
					double totalSeconds = this.stopwatch.Elapsed.TotalSeconds;
					if (totalSeconds > 0.1)
					{
						string text = (this.preHookGcCount == GC.CollectionCount(0)) ? string.Empty : " [GARBAGE COLLECT]";
						Interface.Oxide.LogWarning(string.Format("Calling '{0}' on '{1} v{2}' took {3:0}ms{4}", new object[]
						{
							hook,
							this.Name,
							this.Version,
							totalSeconds * 1000.0,
							text
						}), new object[0]);
					}
					this.stopwatch.Reset();
					double num2 = this.sum + totalSeconds;
					double num3 = (double)num + totalSeconds;
					if (num3 - (double)this.averageAt > 10.0)
					{
						num2 /= num3 - (double)this.averageAt;
						if (num2 > 0.1)
						{
							string text2 = (this.preHookGcCount == GC.CollectionCount(0)) ? string.Empty : " [GARBAGE COLLECT]";
							Interface.Oxide.LogWarning(string.Format("Calling '{0}' on '{1} v{2}' took average {3:0}ms{4}", new object[]
							{
								hook,
								this.Name,
								this.Version,
								this.sum * 1000.0,
								text2
							}), new object[0]);
						}
						this.sum = 0.0;
						this.averageAt = 0f;
					}
					else
					{
						this.sum = num2;
					}
				}
			}
			return result;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008990 File Offset: 0x00006B90
		public object Call(string hook, params object[] args)
		{
			return this.CallHook(hook, args);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000899A File Offset: 0x00006B9A
		public T Call<T>(string hook, params object[] args)
		{
			return (T)((object)Convert.ChangeType(this.CallHook(hook, args), typeof(T)));
		}

		// Token: 0x06000188 RID: 392
		protected abstract object OnCallHook(string hook, object[] args);

		// Token: 0x06000189 RID: 393 RVA: 0x000089B8 File Offset: 0x00006BB8
		public void RaiseError(string message)
		{
			PluginError onError = this.OnError;
			if (onError == null)
			{
				return;
			}
			onError(this, message);
		}

		// Token: 0x0600018A RID: 394 RVA: 0x000089CC File Offset: 0x00006BCC
		public void TrackStart()
		{
			if (this.IsCorePlugin || this.nestcount > 0)
			{
				return;
			}
			Stopwatch stopwatch = this.trackStopwatch;
			if (stopwatch.IsRunning)
			{
				return;
			}
			stopwatch.Start();
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00008A04 File Offset: 0x00006C04
		public void TrackEnd()
		{
			if (this.IsCorePlugin || this.nestcount > 0)
			{
				return;
			}
			Stopwatch stopwatch = this.trackStopwatch;
			if (!stopwatch.IsRunning)
			{
				return;
			}
			stopwatch.Stop();
			this.TotalHookTime += stopwatch.Elapsed.TotalSeconds;
			stopwatch.Reset();
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00008A5C File Offset: 0x00006C5C
		protected virtual void LoadConfig()
		{
			this.Config = new DynamicConfigFile(Path.Combine(this.Manager.ConfigPath, this.Name + ".json"));
			if (!this.Config.Exists(null))
			{
				this.LoadDefaultConfig();
				this.SaveConfig();
			}
			try
			{
				this.Config.Load(null);
			}
			catch (Exception ex)
			{
				this.RaiseError("Failed to load config file (is the config file corrupt?) (" + ex.Message + ")");
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00008AEC File Offset: 0x00006CEC
		protected virtual void LoadDefaultConfig()
		{
			this.CallHook("LoadDefaultConfig", null);
		}

		// Token: 0x0600018E RID: 398 RVA: 0x00008AFC File Offset: 0x00006CFC
		protected virtual void SaveConfig()
		{
			if (this.Config == null)
			{
				return;
			}
			try
			{
				this.Config.Save(null);
			}
			catch (Exception ex)
			{
				this.RaiseError("Failed to save config file (does the config have illegal objects in it?) (" + ex.Message + ")");
			}
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00008B50 File Offset: 0x00006D50
		protected virtual void LoadDefaultMessages()
		{
			this.CallHook("LoadDefaultMessages", null);
		}

		// Token: 0x06000190 RID: 400 RVA: 0x00008B5F File Offset: 0x00006D5F
		public void AddCovalenceCommand(string command, string callback, string perm = null)
		{
			string[] commands = new string[]
			{
				command
			};
			string[] perms;
			if (!string.IsNullOrEmpty(perm))
			{
				(perms = new string[1])[0] = perm;
			}
			else
			{
				perms = null;
			}
			this.AddCovalenceCommand(commands, callback, perms);
		}

		// Token: 0x06000191 RID: 401 RVA: 0x00008B87 File Offset: 0x00006D87
		public void AddCovalenceCommand(string[] commands, string callback, string perm)
		{
			string[] perms;
			if (!string.IsNullOrEmpty(perm))
			{
				(perms = new string[1])[0] = perm;
			}
			else
			{
				perms = null;
			}
			this.AddCovalenceCommand(commands, callback, perms);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00008BA8 File Offset: 0x00006DA8
		public void AddCovalenceCommand(string[] commands, string callback, string[] perms = null)
		{
			this.AddCovalenceCommand(commands, perms, delegate(IPlayer caller, string command, string[] args)
			{
				this.CallHook(callback, new object[]
				{
					caller,
					command,
					args
				});
				return true;
			});
			Covalence library = Interface.Oxide.GetLibrary<Covalence>(null);
			foreach (string command2 in commands)
			{
				library.RegisterCommand(command2, this, new CommandCallback(this.CovalenceCommandCallback));
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00008C14 File Offset: 0x00006E14
		protected void AddCovalenceCommand(string[] commands, string[] perms, CommandCallback callback)
		{
			foreach (string text in commands)
			{
				if (this.commandInfos.ContainsKey(text.ToLowerInvariant()))
				{
					Interface.Oxide.LogWarning("Covalence command alias already exists: {0}", new object[]
					{
						text
					});
				}
				else
				{
					this.commandInfos.Add(text.ToLowerInvariant(), new Plugin.CommandInfo(commands, perms, callback));
				}
			}
			if (perms == null)
			{
				return;
			}
			foreach (string text2 in perms)
			{
				if (!this.permission.PermissionExists(text2, null))
				{
					this.permission.RegisterPermission(text2, this);
				}
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00008CB0 File Offset: 0x00006EB0
		private void RegisterWithCovalence()
		{
			Covalence library = Interface.Oxide.GetLibrary<Covalence>(null);
			foreach (KeyValuePair<string, Plugin.CommandInfo> keyValuePair in this.commandInfos)
			{
				library.RegisterCommand(keyValuePair.Key, this, new CommandCallback(this.CovalenceCommandCallback));
			}
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00008D1C File Offset: 0x00006F1C
		private bool CovalenceCommandCallback(IPlayer caller, string cmd, string[] args)
		{
			Plugin.CommandInfo commandInfo;
			if (!this.commandInfos.TryGetValue(cmd, out commandInfo))
			{
				return false;
			}
			if (caller == null)
			{
				Interface.Oxide.LogWarning("Plugin.CovalenceCommandCallback received null as the caller (bad game Covalence bindings?)", new object[0]);
				return false;
			}
			if (commandInfo.PermissionsRequired != null)
			{
				foreach (string perm in commandInfo.PermissionsRequired)
				{
					if (!caller.HasPermission(perm) && !caller.IsServer && (!caller.IsAdmin || !this.IsCorePlugin))
					{
						caller.Message("You don't have permission to use the command '" + cmd + "'!");
						return true;
					}
				}
			}
			commandInfo.Callback(caller, cmd, args);
			return true;
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00008DC0 File Offset: 0x00006FC0
		private void UnregisterWithCovalence()
		{
			Covalence library = Interface.Oxide.GetLibrary<Covalence>(null);
			foreach (KeyValuePair<string, Plugin.CommandInfo> keyValuePair in this.commandInfos)
			{
				library.UnregisterCommand(keyValuePair.Key, this);
			}
		}

		// Token: 0x04000092 RID: 146
		private string name;

		// Token: 0x0400009B RID: 155
		private bool isCorePlugin;

		// Token: 0x0400009F RID: 159
		public PluginManagerEvent OnAddedToManager = new PluginManagerEvent();

		// Token: 0x040000A0 RID: 160
		public PluginManagerEvent OnRemovedFromManager = new PluginManagerEvent();

		// Token: 0x040000A3 RID: 163
		private Stopwatch trackStopwatch = new Stopwatch();

		// Token: 0x040000A4 RID: 164
		private Stopwatch stopwatch = new Stopwatch();

		// Token: 0x040000A5 RID: 165
		private float averageAt;

		// Token: 0x040000A6 RID: 166
		private double sum;

		// Token: 0x040000A7 RID: 167
		private int preHookGcCount;

		// Token: 0x040000A8 RID: 168
		protected int nestcount;

		// Token: 0x040000A9 RID: 169
		private IDictionary<string, Plugin.CommandInfo> commandInfos;

		// Token: 0x040000AA RID: 170
		private Permission permission = Interface.Oxide.GetLibrary<Permission>(null);

		// Token: 0x0200007A RID: 122
		private class CommandInfo
		{
			// Token: 0x060003EF RID: 1007 RVA: 0x0000FCA6 File Offset: 0x0000DEA6
			public CommandInfo(string[] names, string[] perms, CommandCallback callback)
			{
				this.Names = names;
				this.PermissionsRequired = perms;
				this.Callback = callback;
			}

			// Token: 0x040001A4 RID: 420
			public readonly string[] Names;

			// Token: 0x040001A5 RID: 421
			public readonly string[] PermissionsRequired;

			// Token: 0x040001A6 RID: 422
			public readonly CommandCallback Callback;
		}
	}
}
