using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Newtonsoft.Json;
using Oxide.Core.Configuration;
using Oxide.Core.Extensions;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Logging;
using Oxide.Core.Plugins;
using Oxide.Core.Plugins.Watchers;
using Oxide.Core.RemoteConsole;
using Oxide.Core.ServerConsole;
using WebSocketSharp;

namespace Oxide.Core
{
	// Token: 0x02000012 RID: 18
	public sealed class OxideMod
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000077 RID: 119 RVA: 0x00004383 File Offset: 0x00002583
		// (set) Token: 0x06000078 RID: 120 RVA: 0x0000438B File Offset: 0x0000258B
		public CompoundLogger RootLogger { get; private set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000079 RID: 121 RVA: 0x00004394 File Offset: 0x00002594
		// (set) Token: 0x0600007A RID: 122 RVA: 0x0000439C File Offset: 0x0000259C
		public PluginManager RootPluginManager { get; private set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000043A5 File Offset: 0x000025A5
		// (set) Token: 0x0600007C RID: 124 RVA: 0x000043AD File Offset: 0x000025AD
		public DataFileSystem DataFileSystem { get; private set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600007D RID: 125 RVA: 0x000043B6 File Offset: 0x000025B6
		// (set) Token: 0x0600007E RID: 126 RVA: 0x000043BE File Offset: 0x000025BE
		public string RootDirectory { get; private set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600007F RID: 127 RVA: 0x000043C7 File Offset: 0x000025C7
		// (set) Token: 0x06000080 RID: 128 RVA: 0x000043CF File Offset: 0x000025CF
		public string ExtensionDirectory { get; private set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000081 RID: 129 RVA: 0x000043D8 File Offset: 0x000025D8
		// (set) Token: 0x06000082 RID: 130 RVA: 0x000043E0 File Offset: 0x000025E0
		public string InstanceDirectory { get; private set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000083 RID: 131 RVA: 0x000043E9 File Offset: 0x000025E9
		// (set) Token: 0x06000084 RID: 132 RVA: 0x000043F1 File Offset: 0x000025F1
		public string PluginDirectory { get; private set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000085 RID: 133 RVA: 0x000043FA File Offset: 0x000025FA
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00004402 File Offset: 0x00002602
		public string ConfigDirectory { get; private set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000087 RID: 135 RVA: 0x0000440B File Offset: 0x0000260B
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00004413 File Offset: 0x00002613
		public string DataDirectory { get; private set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000089 RID: 137 RVA: 0x0000441C File Offset: 0x0000261C
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00004424 File Offset: 0x00002624
		public string LangDirectory { get; private set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600008B RID: 139 RVA: 0x0000442D File Offset: 0x0000262D
		// (set) Token: 0x0600008C RID: 140 RVA: 0x00004435 File Offset: 0x00002635
		public string LogDirectory { get; private set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600008D RID: 141 RVA: 0x0000443E File Offset: 0x0000263E
		public float Now
		{
			get
			{
				return this.getTimeSinceStartup();
			}
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000444B File Offset: 0x0000264B
		// (set) Token: 0x0600008F RID: 143 RVA: 0x00004453 File Offset: 0x00002653
		public bool IsShuttingDown { get; private set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000090 RID: 144 RVA: 0x0000445C File Offset: 0x0000265C
		// (set) Token: 0x06000091 RID: 145 RVA: 0x00004464 File Offset: 0x00002664
		public OxideConfig Config { get; private set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000092 RID: 146 RVA: 0x0000446D File Offset: 0x0000266D
		// (set) Token: 0x06000093 RID: 147 RVA: 0x00004475 File Offset: 0x00002675
		public bool HasLoadedCorePlugins { get; private set; }

		// Token: 0x06000094 RID: 148 RVA: 0x0000447E File Offset: 0x0000267E
		public OxideMod(NativeDebugCallback debugCallback)
		{
			this.debugCallback = debugCallback;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000044B0 File Offset: 0x000026B0
		public void Load()
		{
			this.RootDirectory = Environment.CurrentDirectory;
			if (this.RootDirectory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
			{
				this.RootDirectory = AppDomain.CurrentDomain.BaseDirectory;
			}
			if (this.RootDirectory == null)
			{
				throw new Exception("RootDirectory is null");
			}
			this.InstanceDirectory = Path.Combine(this.RootDirectory, "oxide");
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			JsonConvert.DefaultSettings = (() => new JsonSerializerSettings
			{
				Culture = CultureInfo.InvariantCulture
			});
			this.CommandLine = new CommandLine(Environment.GetCommandLineArgs());
			if (this.CommandLine.HasVariable("oxide.directory"))
			{
				string text;
				string format;
				this.CommandLine.GetArgument("oxide.directory", out text, out format);
				if (string.IsNullOrEmpty(text) || this.CommandLine.HasVariable(text))
				{
					this.InstanceDirectory = Path.Combine(this.RootDirectory, Utility.CleanPath(string.Format(format, this.CommandLine.GetVariable(text))));
				}
			}
			this.ExtensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (this.ExtensionDirectory == null || !Directory.Exists(this.ExtensionDirectory))
			{
				throw new Exception("Could not identify extension directory");
			}
			if (!Directory.Exists(this.InstanceDirectory))
			{
				Directory.CreateDirectory(this.InstanceDirectory);
			}
			this.ConfigDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("config"));
			if (!Directory.Exists(this.ConfigDirectory))
			{
				Directory.CreateDirectory(this.ConfigDirectory);
			}
			this.DataDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("data"));
			if (!Directory.Exists(this.DataDirectory))
			{
				Directory.CreateDirectory(this.DataDirectory);
			}
			this.LangDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("lang"));
			if (!Directory.Exists(this.LangDirectory))
			{
				Directory.CreateDirectory(this.LangDirectory);
			}
			this.LogDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("logs"));
			if (!Directory.Exists(this.LogDirectory))
			{
				Directory.CreateDirectory(this.LogDirectory);
			}
			this.PluginDirectory = Path.Combine(this.InstanceDirectory, Utility.CleanPath("plugins"));
			if (!Directory.Exists(this.PluginDirectory))
			{
				Directory.CreateDirectory(this.PluginDirectory);
			}
			OxideMod.RegisterLibrarySearchPath(Path.Combine(this.ExtensionDirectory, (IntPtr.Size == 8) ? "x64" : "x86"));
			string text2 = Path.Combine(this.InstanceDirectory, "oxide.config.json");
			if (File.Exists(text2))
			{
				this.Config = ConfigFile.Load<OxideConfig>(text2);
			}
			else
			{
				this.Config = new OxideConfig(text2);
				this.Config.Save(null);
			}
			if (this.CommandLine.HasVariable("nolog"))
			{
				this.LogWarning("Usage of the 'nolog' variable will prevent logging", new object[0]);
			}
			if (this.CommandLine.HasVariable("rcon.port"))
			{
				this.Config.Rcon.Port = Utility.GetNumbers(this.CommandLine.GetVariable("rcon.port"));
			}
			if (this.CommandLine.HasVariable("rcon.password"))
			{
				this.Config.Rcon.Password = this.CommandLine.GetVariable("rcon.password");
			}
			this.RootLogger = new CompoundLogger();
			this.RootLogger.AddLogger(new RotatingFileLogger
			{
				Directory = this.LogDirectory
			});
			if (this.debugCallback != null)
			{
				this.RootLogger.AddLogger(new CallbackLogger(this.debugCallback));
			}
			this.LogInfo("Loading Oxide Core v{0}...", new object[]
			{
				OxideMod.Version
			});
			this.RootPluginManager = new PluginManager(this.RootLogger)
			{
				ConfigPath = this.ConfigDirectory
			};
			this.extensionManager = new ExtensionManager(this.RootLogger);
			this.DataFileSystem = new DataFileSystem(this.DataDirectory);
			this.extensionManager.RegisterLibrary("Covalence", this.covalence = new Covalence());
			this.extensionManager.RegisterLibrary("Global", new Global());
			this.extensionManager.RegisterLibrary("Lang", new Lang());
			this.extensionManager.RegisterLibrary("Permission", this.libperm = new Permission());
			this.extensionManager.RegisterLibrary("Plugins", new Plugins(this.RootPluginManager));
			this.extensionManager.RegisterLibrary("Time", new Time());
			this.extensionManager.RegisterLibrary("Timer", this.libtimer = new Oxide.Core.Libraries.Timer());
			this.extensionManager.RegisterLibrary("WebRequests", new WebRequests());
			this.LogInfo("Loading extensions...", new object[0]);
			this.extensionManager.LoadAllExtensions(this.ExtensionDirectory);
			Cleanup.Run();
			this.covalence.Initialize();
			this.RemoteConsole = new RemoteConsole();
			RemoteConsole remoteConsole = this.RemoteConsole;
			if (remoteConsole != null)
			{
				remoteConsole.Initalize();
			}
			if (this.getTimeSinceStartup == null)
			{
				this.timer = new Stopwatch();
				this.timer.Start();
				this.getTimeSinceStartup = (() => (float)this.timer.Elapsed.TotalSeconds);
				this.LogWarning("A reliable clock is not available, falling back to a clock which may be unreliable on certain hardware", new object[0]);
			}
			foreach (Extension extension in this.extensionManager.GetAllExtensions())
			{
				extension.LoadPluginWatchers(this.PluginDirectory);
			}
			this.LogInfo("Loading plugins...", new object[0]);
			this.LoadAllPlugins(true);
			foreach (PluginChangeWatcher pluginChangeWatcher in this.extensionManager.GetPluginChangeWatchers())
			{
				pluginChangeWatcher.OnPluginSourceChanged += this.watcher_OnPluginSourceChanged;
				pluginChangeWatcher.OnPluginAdded += this.watcher_OnPluginAdded;
				pluginChangeWatcher.OnPluginRemoved += this.watcher_OnPluginRemoved;
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00004AD4 File Offset: 0x00002CD4
		public T GetLibrary<T>(string name = null) where T : Library
		{
			return this.extensionManager.GetLibrary(name ?? typeof(T).Name) as T;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00004AFF File Offset: 0x00002CFF
		public IEnumerable<Extension> GetAllExtensions()
		{
			return this.extensionManager.GetAllExtensions();
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00004B0C File Offset: 0x00002D0C
		public IEnumerable<PluginLoader> GetPluginLoaders()
		{
			return this.extensionManager.GetPluginLoaders();
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00004B19 File Offset: 0x00002D19
		public void LogDebug(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Warning, "[DEBUG] " + format, args);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00004B33 File Offset: 0x00002D33
		public void LogError(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Error, format, args);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00004B43 File Offset: 0x00002D43
		public void LogException(string message, Exception ex)
		{
			this.RootLogger.WriteException(message, ex);
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00004B52 File Offset: 0x00002D52
		public void LogInfo(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Info, format, args);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00004B62 File Offset: 0x00002D62
		public void LogWarning(string format, params object[] args)
		{
			this.RootLogger.Write(LogType.Warning, format, args);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00004B74 File Offset: 0x00002D74
		public void LoadAllPlugins(bool init = false)
		{
			IEnumerable<PluginLoader> enumerable = this.extensionManager.GetPluginLoaders().ToArray<PluginLoader>();
			if (!this.HasLoadedCorePlugins)
			{
				foreach (PluginLoader pluginLoader in enumerable)
				{
					foreach (Type type in pluginLoader.CorePlugins)
					{
						try
						{
							Plugin plugin = (Plugin)Activator.CreateInstance(type);
							plugin.IsCorePlugin = true;
							this.PluginLoaded(plugin);
						}
						catch (Exception ex)
						{
							this.LogException("Could not load core plugin " + type.Name, ex);
						}
					}
				}
				this.HasLoadedCorePlugins = true;
			}
			foreach (PluginLoader pluginLoader2 in enumerable)
			{
				foreach (string name in pluginLoader2.ScanDirectory(this.PluginDirectory))
				{
					this.LoadPlugin(name);
				}
			}
			if (!init)
			{
				return;
			}
			float now = this.Now;
			foreach (PluginLoader pluginLoader3 in this.extensionManager.GetPluginLoaders())
			{
				while (pluginLoader3.LoadingPlugins.Count > 0)
				{
					Thread.Sleep(25);
					this.OnFrame(this.Now - now);
					now = this.Now;
				}
			}
			this.isInitialized = true;
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00004D34 File Offset: 0x00002F34
		public void UnloadAllPlugins(IList<string> skip = null)
		{
			IEnumerable<Plugin> plugins = this.RootPluginManager.GetPlugins();
			Func<Plugin, bool> <>9__0;
			Func<Plugin, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((Plugin p) => !p.IsCorePlugin && (skip == null || !skip.Contains(p.Name))));
			}
			foreach (Plugin plugin in plugins.Where(predicate).ToArray<Plugin>())
			{
				this.UnloadPlugin(plugin.Name);
			}
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00004DA4 File Offset: 0x00002FA4
		public void ReloadAllPlugins(IList<string> skip = null)
		{
			IEnumerable<Plugin> plugins = this.RootPluginManager.GetPlugins();
			Func<Plugin, bool> <>9__0;
			Func<Plugin, bool> predicate;
			if ((predicate = <>9__0) == null)
			{
				predicate = (<>9__0 = ((Plugin p) => !p.IsCorePlugin && (skip == null || !skip.Contains(p.Name))));
			}
			foreach (Plugin plugin in plugins.Where(predicate).ToArray<Plugin>())
			{
				this.ReloadPlugin(plugin.Name);
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00004E14 File Offset: 0x00003014
		public bool LoadPlugin(string name)
		{
			if (this.RootPluginManager.GetPlugin(name) != null)
			{
				return false;
			}
			HashSet<PluginLoader> hashSet = new HashSet<PluginLoader>(from l in this.extensionManager.GetPluginLoaders()
			where l.ScanDirectory(this.PluginDirectory).Contains(name)
			select l);
			if (hashSet.Count == 0)
			{
				this.LogError("Could not load plugin '{0}' (no plugin found with that file name)", new object[]
				{
					name
				});
				return false;
			}
			if (hashSet.Count > 1)
			{
				this.LogError("Could not load plugin '{0}' (multiple plugin with that name)", new object[]
				{
					name
				});
				return false;
			}
			PluginLoader pluginLoader = hashSet.First<PluginLoader>();
			bool result;
			try
			{
				Plugin plugin = pluginLoader.Load(this.PluginDirectory, name);
				if (plugin == null)
				{
					result = true;
				}
				else
				{
					plugin.Loader = pluginLoader;
					this.PluginLoaded(plugin);
					result = true;
				}
			}
			catch (Exception ex)
			{
				this.LogException("Could not load plugin " + name, ex);
				result = false;
			}
			return result;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00004F1C File Offset: 0x0000311C
		public bool PluginLoaded(Plugin plugin)
		{
			plugin.OnError += this.plugin_OnError;
			bool result;
			try
			{
				PluginLoader loader = plugin.Loader;
				if (loader != null)
				{
					loader.PluginErrors.Remove(plugin.Name);
				}
				this.RootPluginManager.AddPlugin(plugin);
				if (plugin.Loader != null && plugin.Loader.PluginErrors.ContainsKey(plugin.Name))
				{
					this.UnloadPlugin(plugin.Name);
					result = false;
				}
				else
				{
					plugin.IsLoaded = true;
					this.CallHook("OnPluginLoaded", new object[]
					{
						plugin
					});
					this.LogInfo("Loaded plugin {0} v{1} by {2}", new object[]
					{
						plugin.Title,
						plugin.Version,
						plugin.Author
					});
					result = true;
				}
			}
			catch (Exception ex)
			{
				if (plugin.Loader != null)
				{
					plugin.Loader.PluginErrors[plugin.Name] = ex.Message;
				}
				this.LogException(string.Format("Could not initialize plugin '{0} v{1}'", plugin.Name, plugin.Version), ex);
				result = false;
			}
			return result;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005048 File Offset: 0x00003248
		public bool UnloadPlugin(string name)
		{
			Plugin plugin = this.RootPluginManager.GetPlugin(name);
			if (plugin == null)
			{
				return false;
			}
			PluginLoader pluginLoader = this.extensionManager.GetPluginLoaders().SingleOrDefault((PluginLoader l) => l.LoadedPlugins.ContainsKey(name));
			if (pluginLoader != null)
			{
				pluginLoader.Unloading(plugin);
			}
			this.RootPluginManager.RemovePlugin(plugin);
			if (plugin.IsLoaded)
			{
				this.CallHook("OnPluginUnloaded", new object[]
				{
					plugin
				});
			}
			plugin.IsLoaded = false;
			this.LogInfo("Unloaded plugin {0} v{1} by {2}", new object[]
			{
				plugin.Title,
				plugin.Version,
				plugin.Author
			});
			return true;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00005108 File Offset: 0x00003308
		public bool ReloadPlugin(string name)
		{
			bool flag = false;
			string directory = this.PluginDirectory;
			if (name.Contains("\\"))
			{
				flag = true;
				string directoryName = Path.GetDirectoryName(name);
				if (directoryName != null)
				{
					directory = Path.Combine(directory, directoryName);
					name = name.Substring(directoryName.Length + 1);
				}
			}
			PluginLoader pluginLoader = this.extensionManager.GetPluginLoaders().FirstOrDefault((PluginLoader l) => l.ScanDirectory(directory).Contains(name));
			if (pluginLoader != null)
			{
				pluginLoader.Reload(directory, name);
				return true;
			}
			if (flag)
			{
				return false;
			}
			this.UnloadPlugin(name);
			this.LoadPlugin(name);
			return true;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000051D2 File Offset: 0x000033D2
		private void plugin_OnError(Plugin sender, string message)
		{
			this.LogError("{0} v{1}: {2}", new object[]
			{
				sender.Name,
				sender.Version,
				message
			});
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00005200 File Offset: 0x00003400
		public bool LoadExtension(string name)
		{
			string path = (!name.EndsWith(".dll")) ? (name + ".dll") : name;
			string text = Path.Combine(this.ExtensionDirectory, path);
			if (!File.Exists(text))
			{
				this.LogError("Could not load extension '" + name + "' (file not found)", new object[0]);
				return false;
			}
			this.extensionManager.LoadExtension(text, false);
			return true;
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000526C File Offset: 0x0000346C
		public bool UnloadExtension(string name)
		{
			string path = (!name.EndsWith(".dll")) ? (name + ".dll") : name;
			string text = Path.Combine(this.ExtensionDirectory, path);
			if (!File.Exists(text))
			{
				this.LogError("Could not unload extension '" + name + "' (file not found)", new object[0]);
				return false;
			}
			this.extensionManager.UnloadExtension(text);
			return true;
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x000052D8 File Offset: 0x000034D8
		public bool ReloadExtension(string name)
		{
			string path = (!name.EndsWith(".dll")) ? (name + ".dll") : name;
			string text = Path.Combine(this.ExtensionDirectory, path);
			if (!File.Exists(text))
			{
				this.LogError("Could not reload extension '" + name + "' (file not found)", new object[0]);
				return false;
			}
			this.extensionManager.ReloadExtension(text);
			return true;
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00005341 File Offset: 0x00003541
		public object CallHook(string hookname, params object[] args)
		{
			PluginManager rootPluginManager = this.RootPluginManager;
			if (rootPluginManager == null)
			{
				return null;
			}
			return rootPluginManager.CallHook(hookname, args);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00005356 File Offset: 0x00003556
		public object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			PluginManager rootPluginManager = this.RootPluginManager;
			if (rootPluginManager == null)
			{
				return null;
			}
			return rootPluginManager.CallDeprecatedHook(oldHook, newHook, expireDate, args);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005370 File Offset: 0x00003570
		public void NextTick(Action callback)
		{
			object obj = this.nextTickLock;
			lock (obj)
			{
				this.nextTickQueue.Add(callback);
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000053B0 File Offset: 0x000035B0
		public void OnFrame(Action<float> callback)
		{
			this.onFrame = (Action<float>)Delegate.Combine(this.onFrame, callback);
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000053CC File Offset: 0x000035CC
		public void OnFrame(float delta)
		{
			if (this.nextTickQueue.Count > 0)
			{
				object obj = this.nextTickLock;
				List<Action> list;
				lock (obj)
				{
					list = this.nextTickQueue;
					this.nextTickQueue = this.lastTickQueue;
					this.lastTickQueue = list;
				}
				for (int i = 0; i < list.Count; i++)
				{
					try
					{
						list[i]();
					}
					catch (Exception ex)
					{
						this.LogException("Exception while calling NextTick callback", ex);
					}
				}
				list.Clear();
			}
			this.libtimer.Update(delta);
			if (this.isInitialized)
			{
				ServerConsole serverConsole = this.ServerConsole;
				if (serverConsole != null)
				{
					serverConsole.Update();
				}
				try
				{
					Action<float> action = this.onFrame;
					if (action != null)
					{
						action(delta);
					}
				}
				catch (Exception ex2)
				{
					this.LogException(ex2.GetType().Name + " while invoke OnFrame in extensions", ex2);
				}
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000054D0 File Offset: 0x000036D0
		public void OnSave()
		{
			this.libperm.SaveData();
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000054E0 File Offset: 0x000036E0
		public void OnShutdown()
		{
			if (!this.IsShuttingDown)
			{
				this.libperm.SaveData();
				this.IsShuttingDown = true;
				this.UnloadAllPlugins(null);
				foreach (Extension extension in this.extensionManager.GetAllExtensions())
				{
					extension.OnShutdown();
				}
				foreach (string name in this.extensionManager.GetLibraries())
				{
					this.extensionManager.GetLibrary(name).Shutdown();
				}
				RemoteConsole remoteConsole = this.RemoteConsole;
				if (remoteConsole != null)
				{
					remoteConsole.Shutdown("Server shutting down", CloseStatusCode.Normal);
				}
				ServerConsole serverConsole = this.ServerConsole;
				if (serverConsole != null)
				{
					serverConsole.OnDisable();
				}
				this.RootLogger.Shutdown();
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000055D8 File Offset: 0x000037D8
		public void RegisterEngineClock(Func<float> method)
		{
			this.getTimeSinceStartup = method;
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x000055E1 File Offset: 0x000037E1
		public bool CheckConsole(bool force = false)
		{
			return ConsoleWindow.Check(force) && this.Config.Console.Enabled;
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x000055FD File Offset: 0x000037FD
		public bool EnableConsole(bool force = false)
		{
			if (this.CheckConsole(force))
			{
				this.ServerConsole = new ServerConsole();
				this.ServerConsole.OnEnable();
				return true;
			}
			return false;
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00005621 File Offset: 0x00003821
		private void watcher_OnPluginSourceChanged(string name)
		{
			this.ReloadPlugin(name);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000562B File Offset: 0x0000382B
		private void watcher_OnPluginAdded(string name)
		{
			this.LoadPlugin(name);
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00005635 File Offset: 0x00003835
		private void watcher_OnPluginRemoved(string name)
		{
			this.UnloadPlugin(name);
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005640 File Offset: 0x00003840
		private static void RegisterLibrarySearchPath(string path)
		{
			switch (Environment.OSVersion.Platform)
			{
			case PlatformID.Win32S:
			case PlatformID.Win32Windows:
			case PlatformID.Win32NT:
			{
				string text = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
				string value = string.IsNullOrEmpty(text) ? path : (text + Path.PathSeparator.ToString() + path);
				Environment.SetEnvironmentVariable("PATH", value);
				OxideMod.SetDllDirectory(path);
				return;
			}
			case PlatformID.WinCE:
			case PlatformID.Xbox:
				break;
			case PlatformID.Unix:
			case PlatformID.MacOSX:
			{
				string text2 = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? string.Empty;
				string value2 = string.IsNullOrEmpty(text2) ? path : (text2 + Path.PathSeparator.ToString() + path);
				Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", value2);
				break;
			}
			default:
				return;
			}
		}

		// Token: 0x060000B7 RID: 183
		[DllImport("kernel32", SetLastError = true)]
		private static extern bool SetDllDirectory(string lpPathName);

		// Token: 0x04000032 RID: 50
		internal static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

		// Token: 0x04000033 RID: 51
		public static readonly VersionNumber Version = new VersionNumber(OxideMod.AssemblyVersion.Major, OxideMod.AssemblyVersion.Minor, OxideMod.AssemblyVersion.Build);

		// Token: 0x04000040 RID: 64
		private ExtensionManager extensionManager;

		// Token: 0x04000041 RID: 65
		public CommandLine CommandLine;

		// Token: 0x04000043 RID: 67
		private Covalence covalence;

		// Token: 0x04000044 RID: 68
		private Permission libperm;

		// Token: 0x04000045 RID: 69
		private Oxide.Core.Libraries.Timer libtimer;

		// Token: 0x04000046 RID: 70
		private Func<float> getTimeSinceStartup;

		// Token: 0x04000047 RID: 71
		private List<Action> nextTickQueue = new List<Action>();

		// Token: 0x04000048 RID: 72
		private List<Action> lastTickQueue = new List<Action>();

		// Token: 0x04000049 RID: 73
		private readonly object nextTickLock = new object();

		// Token: 0x0400004A RID: 74
		private Action<float> onFrame;

		// Token: 0x0400004B RID: 75
		private bool isInitialized;

		// Token: 0x0400004D RID: 77
		public RemoteConsole RemoteConsole;

		// Token: 0x0400004E RID: 78
		public ServerConsole ServerConsole;

		// Token: 0x0400004F RID: 79
		private Stopwatch timer;

		// Token: 0x04000050 RID: 80
		private NativeDebugCallback debugCallback;
	}
}
