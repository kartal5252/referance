using System;
using System.Collections.Generic;
using Oxide.Core.Logging;

namespace Oxide.Core.Plugins
{
	// Token: 0x02000028 RID: 40
	public sealed class PluginManager
	{
		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x0000909D File Offset: 0x0000729D
		// (set) Token: 0x060001A7 RID: 423 RVA: 0x000090A5 File Offset: 0x000072A5
		public Logger Logger { get; private set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x000090AE File Offset: 0x000072AE
		// (set) Token: 0x060001A9 RID: 425 RVA: 0x000090B6 File Offset: 0x000072B6
		public string ConfigPath { get; set; }

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x060001AA RID: 426 RVA: 0x000090C0 File Offset: 0x000072C0
		// (remove) Token: 0x060001AB RID: 427 RVA: 0x000090F8 File Offset: 0x000072F8
		public event PluginEvent OnPluginAdded;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x060001AC RID: 428 RVA: 0x00009130 File Offset: 0x00007330
		// (remove) Token: 0x060001AD RID: 429 RVA: 0x00009168 File Offset: 0x00007368
		public event PluginEvent OnPluginRemoved;

		// Token: 0x060001AE RID: 430 RVA: 0x0000919D File Offset: 0x0000739D
		public PluginManager(Logger logger)
		{
			this.loadedPlugins = new Dictionary<string, Plugin>();
			this.hookSubscriptions = new Dictionary<string, IList<Plugin>>();
			this.Logger = logger;
		}

		// Token: 0x060001AF RID: 431 RVA: 0x000091D8 File Offset: 0x000073D8
		public bool AddPlugin(Plugin plugin)
		{
			if (this.loadedPlugins.ContainsKey(plugin.Name))
			{
				return false;
			}
			this.loadedPlugins.Add(plugin.Name, plugin);
			plugin.HandleAddedToManager(this);
			PluginEvent onPluginAdded = this.OnPluginAdded;
			if (onPluginAdded != null)
			{
				onPluginAdded(plugin);
			}
			return true;
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00009228 File Offset: 0x00007428
		public bool RemovePlugin(Plugin plugin)
		{
			if (!this.loadedPlugins.ContainsKey(plugin.Name))
			{
				return false;
			}
			this.loadedPlugins.Remove(plugin.Name);
			foreach (IList<Plugin> list in this.hookSubscriptions.Values)
			{
				if (list.Contains(plugin))
				{
					list.Remove(plugin);
				}
			}
			plugin.HandleRemovedFromManager(this);
			PluginEvent onPluginRemoved = this.OnPluginRemoved;
			if (onPluginRemoved != null)
			{
				onPluginRemoved(plugin);
			}
			return true;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x000092C8 File Offset: 0x000074C8
		public Plugin GetPlugin(string name)
		{
			Plugin result;
			if (!this.loadedPlugins.TryGetValue(name, out result))
			{
				return null;
			}
			return result;
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x000092E8 File Offset: 0x000074E8
		public IEnumerable<Plugin> GetPlugins()
		{
			return this.loadedPlugins.Values;
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x000092F8 File Offset: 0x000074F8
		internal void SubscribeToHook(string hook, Plugin plugin)
		{
			if (!this.loadedPlugins.ContainsKey(plugin.Name) || (!plugin.IsCorePlugin && hook.StartsWith("I")))
			{
				return;
			}
			IList<Plugin> list;
			if (!this.hookSubscriptions.TryGetValue(hook, out list))
			{
				list = new List<Plugin>();
				this.hookSubscriptions.Add(hook, list);
			}
			if (!list.Contains(plugin))
			{
				list.Add(plugin);
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00009364 File Offset: 0x00007564
		internal void UnsubscribeToHook(string hook, Plugin plugin)
		{
			if (!this.loadedPlugins.ContainsKey(plugin.Name) || (!plugin.IsCorePlugin && hook.StartsWith("I")))
			{
				return;
			}
			IList<Plugin> list;
			if (this.hookSubscriptions.TryGetValue(hook, out list) && list.Contains(plugin))
			{
				list.Remove(plugin);
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x000093BC File Offset: 0x000075BC
		public object CallHook(string hook, params object[] args)
		{
			IList<Plugin> list;
			if (!this.hookSubscriptions.TryGetValue(hook, out list))
			{
				return null;
			}
			if (list.Count == 0)
			{
				return null;
			}
			object[] array = ArrayPool.Get(list.Count);
			int num = 0;
			object obj = null;
			Plugin plugin = null;
			for (int i = 0; i < list.Count; i++)
			{
				object obj2 = list[i].CallHook(hook, args);
				if (obj2 != null)
				{
					array[i] = obj2;
					obj = obj2;
					plugin = list[i];
					num++;
				}
			}
			if (num == 0)
			{
				ArrayPool.Free(array);
				return null;
			}
			if (num > 1 && obj != null)
			{
				this.hookConflicts.Clear();
				for (int j = 0; j < list.Count; j++)
				{
					object obj3 = array[j];
					if (obj3 != null)
					{
						if (obj3.GetType().IsValueType)
						{
							if (!array[j].Equals(obj))
							{
								this.hookConflicts.Add(string.Format("{0} - {1} ({2})", list[j].Name, obj3, obj3.GetType().Name));
							}
						}
						else if (array[j] != obj)
						{
							this.hookConflicts.Add(string.Format("{0} - {1} ({2})", list[j].Name, obj3, obj3.GetType().Name));
						}
					}
				}
				if (this.hookConflicts.Count > 0)
				{
					this.hookConflicts.Add(string.Format("{0} ({1} ({2}))", plugin.Name, obj, obj.GetType().Name));
					this.Logger.Write(LogType.Warning, "Calling hook {0} resulted in a conflict between the following plugins: {1}", new object[]
					{
						hook,
						string.Join(", ", this.hookConflicts.ToArray())
					});
				}
			}
			ArrayPool.Free(array);
			return obj;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00009578 File Offset: 0x00007778
		public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			IList<Plugin> list;
			if (!this.hookSubscriptions.TryGetValue(oldHook, out list))
			{
				return null;
			}
			if (list.Count == 0)
			{
				return null;
			}
			if (expireDate < DateTime.Now)
			{
				return null;
			}
			float now = Interface.Oxide.Now;
			float num;
			if (!this.lastDeprecatedWarningAt.TryGetValue(oldHook, out num) || now - num > 300f)
			{
				this.lastDeprecatedWarningAt[oldHook] = now;
				Interface.Oxide.LogWarning(string.Format("'{0} v{1}' is using deprecated hook '{2}', which will stop working on {3}. Please ask the author to update to '{4}'", new object[]
				{
					list[0].Name,
					list[0].Version,
					oldHook,
					expireDate.ToString("D"),
					newHook
				}), new object[0]);
			}
			return this.CallHook(oldHook, args);
		}

		// Token: 0x040000B4 RID: 180
		private readonly IDictionary<string, Plugin> loadedPlugins;

		// Token: 0x040000B5 RID: 181
		private readonly IDictionary<string, IList<Plugin>> hookSubscriptions;

		// Token: 0x040000B6 RID: 182
		private readonly Dictionary<string, float> lastDeprecatedWarningAt = new Dictionary<string, float>();

		// Token: 0x040000B7 RID: 183
		private readonly List<string> hookConflicts = new List<string>();
	}
}
