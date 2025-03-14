using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Oxide.Core.Libraries;

namespace Oxide.Core.Plugins
{
	// Token: 0x02000026 RID: 38
	public abstract class PluginLoader
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000197 RID: 407 RVA: 0x00008E20 File Offset: 0x00007020
		public ConcurrentHashSet<string> LoadingPlugins { get; } = new ConcurrentHashSet<string>();

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000198 RID: 408 RVA: 0x00008E28 File Offset: 0x00007028
		public Dictionary<string, string> PluginErrors { get; } = new Dictionary<string, string>();

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000199 RID: 409 RVA: 0x00008E30 File Offset: 0x00007030
		public virtual Type[] CorePlugins { get; } = new Type[0];

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x0600019A RID: 410 RVA: 0x00008E38 File Offset: 0x00007038
		public virtual string FileExtension { get; }

		// Token: 0x0600019B RID: 411 RVA: 0x00008E40 File Offset: 0x00007040
		public virtual IEnumerable<string> ScanDirectory(string directory)
		{
			if (this.FileExtension == null || !Directory.Exists(directory))
			{
				yield break;
			}
			IEnumerable<FileInfo> enumerable = from f in new DirectoryInfo(directory).GetFiles("*" + this.FileExtension)
			where (f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
			select f;
			foreach (FileInfo fileInfo in enumerable)
			{
				yield return Utility.GetFileNameWithoutExtension(fileInfo.FullName);
			}
			IEnumerator<FileInfo> enumerator = null;
			yield break;
			yield break;
		}

		// Token: 0x0600019C RID: 412 RVA: 0x00008E58 File Offset: 0x00007058
		public virtual Plugin Load(string directory, string name)
		{
			if (this.LoadingPlugins.Contains(name))
			{
				Interface.Oxide.LogDebug("Load requested for plugin which is already loading: {0}", new object[]
				{
					name
				});
				return null;
			}
			string filename = Path.Combine(directory, name + this.FileExtension);
			Plugin plugin = this.GetPlugin(filename);
			this.LoadingPlugins.Add(plugin.Name);
			Interface.Oxide.NextTick(delegate
			{
				this.LoadPlugin(plugin, false);
			});
			return null;
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008EE9 File Offset: 0x000070E9
		protected virtual Plugin GetPlugin(string filename)
		{
			return null;
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00008EEC File Offset: 0x000070EC
		protected void LoadPlugin(Plugin plugin, bool waitingForAccess = false)
		{
			if (!File.Exists(plugin.Filename))
			{
				this.LoadingPlugins.Remove(plugin.Name);
				Interface.Oxide.LogWarning("Script no longer exists: {0}", new object[]
				{
					plugin.Name
				});
				return;
			}
			try
			{
				plugin.Load();
				Interface.Oxide.UnloadPlugin(plugin.Name);
				this.LoadingPlugins.Remove(plugin.Name);
				Interface.Oxide.PluginLoaded(plugin);
			}
			catch (IOException)
			{
				if (!waitingForAccess)
				{
					Interface.Oxide.LogWarning("Waiting for another application to stop using script: {0}", new object[]
					{
						plugin.Name
					});
				}
				Interface.Oxide.GetLibrary<Timer>(null).Once(0.5f, delegate
				{
					this.LoadPlugin(plugin, true);
				}, null);
			}
			catch (Exception ex)
			{
				this.LoadingPlugins.Remove(plugin.Name);
				Interface.Oxide.LogException("Failed to load plugin " + plugin.Name, ex);
			}
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000904C File Offset: 0x0000724C
		public virtual void Reload(string directory, string name)
		{
			Interface.Oxide.UnloadPlugin(name);
			Interface.Oxide.LoadPlugin(name);
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00009066 File Offset: 0x00007266
		public virtual void Unloading(Plugin plugin)
		{
		}

		// Token: 0x040000AC RID: 172
		public Dictionary<string, Plugin> LoadedPlugins = new Dictionary<string, Plugin>();
	}
}
