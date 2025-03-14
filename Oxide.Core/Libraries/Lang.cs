using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core.Plugins;
using ProtoBuf;

namespace Oxide.Core.Libraries
{
	// Token: 0x02000036 RID: 54
	public class Lang : Library
	{
		// Token: 0x17000042 RID: 66
		// (get) Token: 0x060001FA RID: 506 RVA: 0x0000A150 File Offset: 0x00008350
		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000A153 File Offset: 0x00008353
		public Lang()
		{
			this.langFiles = new Dictionary<string, Dictionary<string, string>>();
			this.langData = (ProtoStorage.Load<Lang.LangData>(new string[]
			{
				"oxide.lang"
			}) ?? new Lang.LangData());
			this.pluginRemovedFromManager = new Dictionary<Plugin, Event.Callback<Plugin, PluginManager>>();
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000A194 File Offset: 0x00008394
		[LibraryFunction("RegisterMessages")]
		public void RegisterMessages(Dictionary<string, string> messages, Plugin plugin, string lang = "en")
		{
			if (messages == null || string.IsNullOrEmpty(lang) || plugin == null)
			{
				return;
			}
			string text = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin.Name);
			Dictionary<string, string> messageFile = this.GetMessageFile(plugin.Name, lang);
			bool flag;
			if (messageFile == null)
			{
				this.langFiles.Remove(text);
				this.AddLangFile(text, messages, plugin);
				flag = true;
			}
			else
			{
				flag = this.MergeMessages(messageFile, messages);
				messages = messageFile;
			}
			if (!flag)
			{
				return;
			}
			if (!Directory.Exists(Path.Combine(Interface.Oxide.LangDirectory, lang)))
			{
				Directory.CreateDirectory(Path.Combine(Interface.Oxide.LangDirectory, lang));
			}
			File.WriteAllText(Path.Combine(Interface.Oxide.LangDirectory, text), JsonConvert.SerializeObject(messages, Formatting.Indented));
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000A250 File Offset: 0x00008450
		[LibraryFunction("GetLanguage")]
		public string GetLanguage(string userId)
		{
			string result;
			if (!string.IsNullOrEmpty(userId) && this.langData.UserData.TryGetValue(userId, out result))
			{
				return result;
			}
			return this.langData.Lang;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000A288 File Offset: 0x00008488
		[LibraryFunction("GetLanguages")]
		public string[] GetLanguages(Plugin plugin = null)
		{
			List<string> list = new List<string>();
			foreach (string text in Directory.GetDirectories(Interface.Oxide.LangDirectory))
			{
				if (Directory.GetFiles(text).Length != 0 && (plugin == null || (plugin != null && File.Exists(Path.Combine(text, plugin.Name + ".json")))))
				{
					list.Add(text.Substring(Interface.Oxide.LangDirectory.Length + 1));
				}
			}
			return list.ToArray();
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000A30C File Offset: 0x0000850C
		[LibraryFunction("GetMessage")]
		public string GetMessage(string key, Plugin plugin, string userId = null)
		{
			if (string.IsNullOrEmpty(key) || plugin == null)
			{
				return key;
			}
			return this.GetMessageKey(key, plugin, this.GetLanguage(userId));
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000A32C File Offset: 0x0000852C
		[LibraryFunction("GetMessages")]
		public Dictionary<string, string> GetMessages(string lang, Plugin plugin)
		{
			if (string.IsNullOrEmpty(lang) || plugin == null)
			{
				return null;
			}
			string text = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin.Name);
			Dictionary<string, string> messageFile;
			if (!this.langFiles.TryGetValue(text, out messageFile))
			{
				messageFile = this.GetMessageFile(plugin.Name, lang);
				if (messageFile == null)
				{
					return null;
				}
				this.AddLangFile(text, messageFile, plugin);
			}
			return messageFile.ToDictionary((KeyValuePair<string, string> k) => k.Key, (KeyValuePair<string, string> v) => v.Value);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000A3D2 File Offset: 0x000085D2
		[LibraryFunction("GetServerLanguage")]
		public string GetServerLanguage()
		{
			return this.langData.Lang;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000A3E0 File Offset: 0x000085E0
		[LibraryFunction("SetLanguage")]
		public void SetLanguage(string lang, string userId)
		{
			if (string.IsNullOrEmpty(lang) || string.IsNullOrEmpty(userId))
			{
				return;
			}
			string value;
			if (this.langData.UserData.TryGetValue(userId, out value) && lang.Equals(value))
			{
				return;
			}
			this.langData.UserData[userId] = lang;
			this.SaveData();
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000A435 File Offset: 0x00008635
		[LibraryFunction("SetServerLanguage")]
		public void SetServerLanguage(string lang)
		{
			if (string.IsNullOrEmpty(lang) || lang.Equals(this.langData.Lang))
			{
				return;
			}
			this.langData.Lang = lang;
			this.SaveData();
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000A468 File Offset: 0x00008668
		private void AddLangFile(string file, Dictionary<string, string> langFile, Plugin plugin)
		{
			this.langFiles.Add(file, langFile);
			if (plugin != null && !this.pluginRemovedFromManager.ContainsKey(plugin))
			{
				this.pluginRemovedFromManager[plugin] = plugin.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.plugin_OnRemovedFromManager));
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000A4B8 File Offset: 0x000086B8
		private Dictionary<string, string> GetMessageFile(string plugin, string lang = "en")
		{
			if (string.IsNullOrEmpty(plugin))
			{
				return null;
			}
			foreach (char oldChar in Path.GetInvalidFileNameChars())
			{
				lang = lang.Replace(oldChar, '_');
			}
			string path = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin);
			string path2 = Path.Combine(Interface.Oxide.LangDirectory, path);
			if (!File.Exists(path2))
			{
				return null;
			}
			return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path2));
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000A534 File Offset: 0x00008734
		private string GetMessageKey(string key, Plugin plugin, string lang = "en")
		{
			string text = string.Format("{0}{1}{2}.json", lang, Path.DirectorySeparatorChar, plugin.Name);
			Dictionary<string, string> dictionary;
			if (!this.langFiles.TryGetValue(text, out dictionary))
			{
				Dictionary<string, string> dictionary2;
				if ((dictionary2 = this.GetMessageFile(plugin.Name, lang)) == null)
				{
					dictionary2 = (this.GetMessageFile(plugin.Name, this.langData.Lang) ?? this.GetMessageFile(plugin.Name, "en"));
				}
				dictionary = dictionary2;
				if (dictionary == null)
				{
					Interface.Oxide.LogWarning("Plugin '" + plugin.Name + "' is using the Lang API but has no messages registered", new object[0]);
					return key;
				}
				Dictionary<string, string> messageFile = this.GetMessageFile(plugin.Name, "en");
				if (messageFile != null && this.MergeMessages(dictionary, messageFile) && File.Exists(Path.Combine(Interface.Oxide.LangDirectory, text)))
				{
					File.WriteAllText(Path.Combine(Interface.Oxide.LangDirectory, text), JsonConvert.SerializeObject(dictionary, Formatting.Indented));
				}
				this.AddLangFile(text, dictionary, plugin);
			}
			string result;
			if (!dictionary.TryGetValue(key, out result))
			{
				return key;
			}
			return result;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000A644 File Offset: 0x00008844
		private bool MergeMessages(Dictionary<string, string> existingMessages, Dictionary<string, string> messages)
		{
			bool result = false;
			foreach (KeyValuePair<string, string> keyValuePair in messages)
			{
				if (!existingMessages.ContainsKey(keyValuePair.Key))
				{
					existingMessages.Add(keyValuePair.Key, keyValuePair.Value);
					result = true;
				}
			}
			if (existingMessages.Count > 0)
			{
				foreach (string key in existingMessages.Keys.ToArray<string>())
				{
					if (!messages.ContainsKey(key))
					{
						existingMessages.Remove(key);
						result = true;
					}
				}
			}
			return result;
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000A6F8 File Offset: 0x000088F8
		private void SaveData()
		{
			ProtoStorage.Save<Lang.LangData>(this.langData, new string[]
			{
				"oxide.lang"
			});
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000A714 File Offset: 0x00008914
		private void plugin_OnRemovedFromManager(Plugin sender, PluginManager manager)
		{
			Event.Callback<Plugin, PluginManager> callback;
			if (this.pluginRemovedFromManager.TryGetValue(sender, out callback))
			{
				callback.Remove();
				this.pluginRemovedFromManager.Remove(sender);
			}
			foreach (string arg in this.GetLanguages(sender))
			{
				this.langFiles.Remove(string.Format("{0}{1}{2}.json", arg, Path.DirectorySeparatorChar, sender.Name));
			}
		}

		// Token: 0x040000D2 RID: 210
		private const string defaultLang = "en";

		// Token: 0x040000D3 RID: 211
		private readonly Lang.LangData langData;

		// Token: 0x040000D4 RID: 212
		private readonly Dictionary<string, Dictionary<string, string>> langFiles;

		// Token: 0x040000D5 RID: 213
		private readonly Dictionary<Plugin, Event.Callback<Plugin, PluginManager>> pluginRemovedFromManager;

		// Token: 0x02000084 RID: 132
		[ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
		private class LangData
		{
			// Token: 0x040001C0 RID: 448
			public string Lang = "en";

			// Token: 0x040001C1 RID: 449
			public readonly Dictionary<string, string> UserData = new Dictionary<string, string>();
		}
	}
}
