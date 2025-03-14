using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Oxide.Core.Libraries;
using Oxide.Core.Logging;
using Oxide.Core.Plugins;
using Oxide.Core.Plugins.Watchers;

namespace Oxide.Core.Extensions
{
	// Token: 0x02000053 RID: 83
	public sealed class ExtensionManager
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000328 RID: 808 RVA: 0x0000D4BE File Offset: 0x0000B6BE
		// (set) Token: 0x06000329 RID: 809 RVA: 0x0000D4C6 File Offset: 0x0000B6C6
		public CompoundLogger Logger { get; private set; }

		// Token: 0x0600032A RID: 810 RVA: 0x0000D4CF File Offset: 0x0000B6CF
		public ExtensionManager(CompoundLogger logger)
		{
			this.Logger = logger;
			this.extensions = new List<Extension>();
			this.pluginloaders = new List<PluginLoader>();
			this.libraries = new Dictionary<string, Library>();
			this.changewatchers = new List<PluginChangeWatcher>();
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0000D50A File Offset: 0x0000B70A
		public void RegisterPluginLoader(PluginLoader loader)
		{
			this.pluginloaders.Add(loader);
		}

		// Token: 0x0600032C RID: 812 RVA: 0x0000D518 File Offset: 0x0000B718
		public IEnumerable<PluginLoader> GetPluginLoaders()
		{
			return this.pluginloaders;
		}

		// Token: 0x0600032D RID: 813 RVA: 0x0000D520 File Offset: 0x0000B720
		public void RegisterLibrary(string name, Library library)
		{
			if (this.libraries.ContainsKey(name))
			{
				Interface.Oxide.LogError("An extension tried to register an already registered library: " + name, new object[0]);
				return;
			}
			this.libraries[name] = library;
		}

		// Token: 0x0600032E RID: 814 RVA: 0x0000D559 File Offset: 0x0000B759
		public IEnumerable<string> GetLibraries()
		{
			return this.libraries.Keys;
		}

		// Token: 0x0600032F RID: 815 RVA: 0x0000D568 File Offset: 0x0000B768
		public Library GetLibrary(string name)
		{
			Library result;
			if (this.libraries.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06000330 RID: 816 RVA: 0x0000D588 File Offset: 0x0000B788
		public void RegisterPluginChangeWatcher(PluginChangeWatcher watcher)
		{
			this.changewatchers.Add(watcher);
		}

		// Token: 0x06000331 RID: 817 RVA: 0x0000D596 File Offset: 0x0000B796
		public IEnumerable<PluginChangeWatcher> GetPluginChangeWatchers()
		{
			return this.changewatchers;
		}

		// Token: 0x06000332 RID: 818 RVA: 0x0000D5A0 File Offset: 0x0000B7A0
		public void LoadExtension(string filename, bool forced)
		{
			string fileNameWithoutExtension = Utility.GetFileNameWithoutExtension(filename);
			if (this.extensions.Any((Extension x) => x.Filename == filename))
			{
				this.Logger.Write(LogType.Error, "Failed to load extension '" + fileNameWithoutExtension + "': extension already loaded.", new object[0]);
				return;
			}
			try
			{
				Assembly assembly = Assembly.Load(File.ReadAllBytes(filename));
				Type typeFromHandle = typeof(Extension);
				Type type = null;
				foreach (Type type2 in assembly.GetExportedTypes())
				{
					if (typeFromHandle.IsAssignableFrom(type2))
					{
						type = type2;
						break;
					}
				}
				if (type == null)
				{
					this.Logger.Write(LogType.Error, "Failed to load extension {0} ({1})", new object[]
					{
						fileNameWithoutExtension,
						"Specified assembly does not implement an Extension class"
					});
				}
				else
				{
					Extension extension = Activator.CreateInstance(type, new object[]
					{
						this
					}) as Extension;
					if (extension != null)
					{
						if (!forced)
						{
							if (extension.IsCoreExtension || extension.IsGameExtension)
							{
								this.Logger.Write(LogType.Error, "Failed to load extension '" + fileNameWithoutExtension + "': you may not hotload Core or Game extensions.", new object[0]);
								return;
							}
							if (!extension.SupportsReloading)
							{
								this.Logger.Write(LogType.Error, "Failed to load extension '" + fileNameWithoutExtension + "': this extension does not support reloading.", new object[0]);
								return;
							}
						}
						extension.Filename = filename;
						extension.Load();
						this.extensions.Add(extension);
						this.Logger.Write(LogType.Info, "Loaded extension {0} v{1} by {2}", new object[]
						{
							extension.Name,
							extension.Version,
							extension.Author
						});
					}
				}
			}
			catch (Exception ex)
			{
				this.Logger.WriteException("Failed to load extension " + fileNameWithoutExtension, ex);
				RemoteLogger.Exception("Failed to load extension " + fileNameWithoutExtension, ex);
			}
		}

		// Token: 0x06000333 RID: 819 RVA: 0x0000D7AC File Offset: 0x0000B9AC
		public void UnloadExtension(string filename)
		{
			string fileNameWithoutExtension = Utility.GetFileNameWithoutExtension(filename);
			Extension extension = this.extensions.SingleOrDefault((Extension x) => x.Filename == filename);
			if (extension == null)
			{
				this.Logger.Write(LogType.Error, "Failed to unload extension '" + fileNameWithoutExtension + "': extension not loaded.", new object[0]);
				return;
			}
			if (extension.IsCoreExtension || extension.IsGameExtension)
			{
				this.Logger.Write(LogType.Error, "Failed to unload extension '" + fileNameWithoutExtension + "': you may not unload Core or Game extensions.", new object[0]);
				return;
			}
			if (!extension.SupportsReloading)
			{
				this.Logger.Write(LogType.Error, "Failed to unload extension '" + fileNameWithoutExtension + "': this extension doesn't support reloading.", new object[0]);
				return;
			}
			extension.Unload();
			this.extensions.Remove(extension);
			this.Logger.Write(LogType.Info, string.Format("Unloaded extension {0} v{1} by {2}", extension.Name, extension.Version, extension.Author), new object[0]);
		}

		// Token: 0x06000334 RID: 820 RVA: 0x0000D8B4 File Offset: 0x0000BAB4
		public void ReloadExtension(string filename)
		{
			string name = Utility.GetFileNameWithoutExtension(filename);
			Extension extension = this.extensions.SingleOrDefault((Extension x) => Utility.GetFileNameWithoutExtension(x.Filename) == name);
			if (extension == null)
			{
				this.LoadExtension(filename, false);
				return;
			}
			if (extension.IsCoreExtension || extension.IsGameExtension)
			{
				this.Logger.Write(LogType.Error, "Failed to unload extension '" + name + "': you may not unload Core or Game extensions.", new object[0]);
				return;
			}
			if (!extension.SupportsReloading)
			{
				this.Logger.Write(LogType.Error, "Failed to reload extension '" + name + "': this extension doesn't support reloading.", new object[0]);
				return;
			}
			this.UnloadExtension(filename);
			this.LoadExtension(filename, false);
		}

		// Token: 0x06000335 RID: 821 RVA: 0x0000D970 File Offset: 0x0000BB70
		public void LoadAllExtensions(string directory)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			string[] array = new string[]
			{
				"Oxide.CSharp",
				"Oxide.JavaScript",
				"Oxide.Lua",
				"Oxide.MySql",
				"Oxide.Python",
				"Oxide.SQLite",
				"Oxide.Unity"
			};
			string[] array2 = new string[]
			{
				"Oxide.Blackwake",
				"Oxide.Blockstorm",
				"Oxide.FortressCraft",
				"Oxide.FromTheDepths",
				"Oxide.GangBeasts",
				"Oxide.Hurtworld",
				"Oxide.InterstellarRift",
				"Oxide.MedievalEngineers",
				"Oxide.Nomad",
				"Oxide.PlanetExplorers",
				"Oxide.ReignOfKings",
				"Oxide.Rust",
				"Oxide.RustLegacy",
				"Oxide.SavageLands",
				"Oxide.SevenDaysToDie",
				"Oxide.SpaceEngineers",
				"Oxide.TheForest",
				"Oxide.Terraria",
				"Oxide.Unturned"
			};
			string[] files = Directory.GetFiles(directory, "Oxide.*.dll");
			foreach (string text in from e in files
			where !e.EndsWith("Oxide.Core.dll") && !e.EndsWith("Oxide.References.dll")
			select e)
			{
				if (text.Contains("Oxide.Core.") && Array.IndexOf<string>(files, text.Replace(".Core", "")) != -1)
				{
					Cleanup.Add(text);
				}
				else if (text.Contains("Oxide.Ext.") && Array.IndexOf<string>(files, text.Replace(".Ext", "")) != -1)
				{
					Cleanup.Add(text);
				}
				else if (text.Contains("Oxide.Game."))
				{
					Cleanup.Add(text);
				}
				else if (array.Contains(text.Basename(null)))
				{
					list.Add(text);
				}
				else if (array2.Contains(text.Basename(null)))
				{
					list2.Add(text);
				}
				else
				{
					list3.Add(text);
				}
			}
			foreach (string path in list)
			{
				this.LoadExtension(Path.Combine(directory, path), true);
			}
			foreach (string path2 in list2)
			{
				this.LoadExtension(Path.Combine(directory, path2), true);
			}
			foreach (string path3 in list3)
			{
				this.LoadExtension(Path.Combine(directory, path3), true);
			}
			foreach (Extension extension in this.extensions.ToArray<Extension>())
			{
				try
				{
					extension.OnModLoad();
				}
				catch (Exception ex)
				{
					this.extensions.Remove(extension);
					this.Logger.WriteException(string.Format("Failed OnModLoad extension {0} v{1}", extension.Name, extension.Version), ex);
					RemoteLogger.Exception(string.Format("Failed OnModLoad extension {0} v{1}", extension.Name, extension.Version), ex);
				}
			}
		}

		// Token: 0x06000336 RID: 822 RVA: 0x0000DD18 File Offset: 0x0000BF18
		public IEnumerable<Extension> GetAllExtensions()
		{
			return this.extensions;
		}

		// Token: 0x06000337 RID: 823 RVA: 0x0000DD20 File Offset: 0x0000BF20
		public bool IsExtensionPresent(string name)
		{
			return this.extensions.Any((Extension e) => e.Name == name);
		}

		// Token: 0x06000338 RID: 824 RVA: 0x0000DD54 File Offset: 0x0000BF54
		public Extension GetExtension(string name)
		{
			Extension result;
			try
			{
				result = this.extensions.Single((Extension e) => e.Name == name);
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0400012C RID: 300
		private IList<Extension> extensions;

		// Token: 0x0400012D RID: 301
		private const string extSearchPattern = "Oxide.*.dll";

		// Token: 0x0400012F RID: 303
		private IList<PluginLoader> pluginloaders;

		// Token: 0x04000130 RID: 304
		private IDictionary<string, Library> libraries;

		// Token: 0x04000131 RID: 305
		private IList<PluginChangeWatcher> changewatchers;
	}
}
