using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using Oxide.Core.Libraries;

namespace Oxide.Core.Plugins.Watchers
{
	// Token: 0x0200002A RID: 42
	public sealed class FSWatcher : PluginChangeWatcher
	{
		// Token: 0x060001BC RID: 444 RVA: 0x00009680 File Offset: 0x00007880
		public FSWatcher(string directory, string filter)
		{
			this.watchedPlugins = new HashSet<string>();
			this.changeQueue = new Dictionary<string, FSWatcher.QueuedChange>();
			this.timers = Interface.Oxide.GetLibrary<Timer>(null);
			if (Interface.Oxide.Config.Options.PluginWatchers)
			{
				this.LoadWatcher(directory, filter);
				return;
			}
			Interface.Oxide.LogWarning("Automatic plugin reloading and unloading has been disabled", new object[0]);
		}

		// Token: 0x060001BD RID: 445 RVA: 0x000096F0 File Offset: 0x000078F0
		[PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
		private void LoadWatcher(string directory, string filter)
		{
			this.watcher = new FileSystemWatcher(directory, filter);
			this.watcher.Changed += this.watcher_Changed;
			this.watcher.Created += this.watcher_Changed;
			this.watcher.Deleted += this.watcher_Changed;
			this.watcher.Error += this.watcher_Error;
			this.watcher.NotifyFilter = NotifyFilters.LastWrite;
			this.watcher.IncludeSubdirectories = true;
			this.watcher.EnableRaisingEvents = true;
			GC.KeepAlive(this.watcher);
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00009796 File Offset: 0x00007996
		public void AddMapping(string name)
		{
			this.watchedPlugins.Add(name);
		}

		// Token: 0x060001BF RID: 447 RVA: 0x000097A4 File Offset: 0x000079A4
		public void RemoveMapping(string name)
		{
			this.watchedPlugins.Remove(name);
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x000097B4 File Offset: 0x000079B4
		private void watcher_Changed(object sender, FileSystemEventArgs e)
		{
			FileSystemWatcher fileSystemWatcher = (FileSystemWatcher)sender;
			int length = e.FullPath.Length - fileSystemWatcher.Path.Length - Path.GetExtension(e.Name).Length - 1;
			string sub_path = e.FullPath.Substring(fileSystemWatcher.Path.Length + 1, length);
			FSWatcher.QueuedChange change;
			if (!this.changeQueue.TryGetValue(sub_path, out change))
			{
				change = new FSWatcher.QueuedChange();
				this.changeQueue[sub_path] = change;
			}
			Timer.TimerInstance timer = change.timer;
			if (timer != null)
			{
				timer.Destroy();
			}
			change.timer = null;
			switch (e.ChangeType)
			{
			case WatcherChangeTypes.Created:
				if (change.type == WatcherChangeTypes.Deleted)
				{
					change.type = WatcherChangeTypes.Changed;
				}
				else
				{
					change.type = WatcherChangeTypes.Created;
				}
				break;
			case WatcherChangeTypes.Deleted:
				if (change.type == WatcherChangeTypes.Created)
				{
					this.changeQueue.Remove(sub_path);
					return;
				}
				change.type = WatcherChangeTypes.Deleted;
				break;
			case WatcherChangeTypes.Changed:
				if (change.type != WatcherChangeTypes.Created)
				{
					change.type = WatcherChangeTypes.Changed;
				}
				break;
			}
			Action <>9__1;
			Interface.Oxide.NextTick(delegate
			{
				FSWatcher.QueuedChange change;
				Timer.TimerInstance timer2 = change.timer;
				if (timer2 != null)
				{
					timer2.Destroy();
				}
				change = change;
				Timer timer3 = this.timers;
				float delay = 0.2f;
				Action callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate()
					{
						change.timer = null;
						this.changeQueue.Remove(sub_path);
						if (Regex.Match(sub_path, "include\\\\", RegexOptions.IgnoreCase).Success)
						{
							if (change.type == WatcherChangeTypes.Created || change.type == WatcherChangeTypes.Changed)
							{
								this.FirePluginSourceChanged(sub_path);
							}
							return;
						}
						switch (change.type)
						{
						case WatcherChangeTypes.Created:
							this.FirePluginAdded(sub_path);
							return;
						case WatcherChangeTypes.Deleted:
							if (this.watchedPlugins.Contains(sub_path))
							{
								this.FirePluginRemoved(sub_path);
							}
							break;
						case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted:
							break;
						case WatcherChangeTypes.Changed:
							if (this.watchedPlugins.Contains(sub_path))
							{
								this.FirePluginSourceChanged(sub_path);
								return;
							}
							this.FirePluginAdded(sub_path);
							return;
						default:
							return;
						}
					});
				}
				change.timer = timer3.Once(delay, callback, null);
			});
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00009928 File Offset: 0x00007B28
		private void watcher_Error(object sender, ErrorEventArgs e)
		{
			Interface.Oxide.NextTick(delegate
			{
				Interface.Oxide.LogError("FSWatcher error: {0}", new object[]
				{
					e.GetException()
				});
				RemoteLogger.Exception("FSWatcher error", e.GetException());
			});
		}

		// Token: 0x040000BA RID: 186
		private FileSystemWatcher watcher;

		// Token: 0x040000BB RID: 187
		private ICollection<string> watchedPlugins;

		// Token: 0x040000BC RID: 188
		private Dictionary<string, FSWatcher.QueuedChange> changeQueue;

		// Token: 0x040000BD RID: 189
		private Timer timers;

		// Token: 0x02000080 RID: 128
		private class QueuedChange
		{
			// Token: 0x040001B6 RID: 438
			internal WatcherChangeTypes type;

			// Token: 0x040001B7 RID: 439
			internal Timer.TimerInstance timer;
		}
	}
}
