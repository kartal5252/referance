using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core.Configuration;

namespace Oxide.Core
{
	// Token: 0x02000006 RID: 6
	public class DataFileSystem
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002742 File Offset: 0x00000942
		// (set) Token: 0x06000012 RID: 18 RVA: 0x0000274A File Offset: 0x0000094A
		public string Directory { get; private set; }

		// Token: 0x06000013 RID: 19 RVA: 0x00002754 File Offset: 0x00000954
		public DataFileSystem(string directory)
		{
			this.Directory = directory;
			this._datafiles = new Dictionary<string, DynamicConfigFile>();
			KeyValuesConverter item = new KeyValuesConverter();
			new JsonSerializerSettings().Converters.Add(item);
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002790 File Offset: 0x00000990
		public DynamicConfigFile GetFile(string name)
		{
			name = DynamicConfigFile.SanitizeName(name);
			DynamicConfigFile dynamicConfigFile;
			if (this._datafiles.TryGetValue(name, out dynamicConfigFile))
			{
				return dynamicConfigFile;
			}
			dynamicConfigFile = new DynamicConfigFile(Path.Combine(this.Directory, name + ".json"));
			this._datafiles.Add(name, dynamicConfigFile);
			return dynamicConfigFile;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x000027E1 File Offset: 0x000009E1
		public bool ExistsDatafile(string name)
		{
			return this.GetFile(name).Exists(null);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000027F0 File Offset: 0x000009F0
		public DynamicConfigFile GetDatafile(string name)
		{
			DynamicConfigFile file = this.GetFile(name);
			if (file.Exists(null))
			{
				file.Load(null);
			}
			else
			{
				file.Save(null);
			}
			return file;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000281F File Offset: 0x00000A1F
		public string[] GetFiles(string path = "", string searchPattern = "*")
		{
			return System.IO.Directory.GetFiles(Path.Combine(this.Directory, path), searchPattern);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002833 File Offset: 0x00000A33
		public void SaveDatafile(string name)
		{
			this.GetFile(name).Save(null);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002844 File Offset: 0x00000A44
		public T ReadObject<T>(string name)
		{
			if (!this.ExistsDatafile(name))
			{
				T t = Activator.CreateInstance<T>();
				this.WriteObject<T>(name, t, false);
				return t;
			}
			return this.GetFile(name).ReadObject<T>(null);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002878 File Offset: 0x00000A78
		public void WriteObject<T>(string name, T Object, bool sync = false)
		{
			this.GetFile(name).WriteObject<T>(Object, sync, null);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000288C File Offset: 0x00000A8C
		public void ForEachObject<T>(string name, Action<T> callback)
		{
			string folder = DynamicConfigFile.SanitizeName(name);
			foreach (DynamicConfigFile dynamicConfigFile in from d in this._datafiles
			where d.Key.StartsWith(folder)
			select d into a
			select a.Value)
			{
				if (callback != null)
				{
					callback(dynamicConfigFile.ReadObject<T>(null));
				}
			}
		}

		// Token: 0x0400000F RID: 15
		private readonly Dictionary<string, DynamicConfigFile> _datafiles;
	}
}
