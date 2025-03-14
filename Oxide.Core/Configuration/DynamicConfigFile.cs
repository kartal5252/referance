using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Oxide.Core.Configuration
{
	// Token: 0x02000058 RID: 88
	public class DynamicConfigFile : ConfigFile, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		// Token: 0x17000093 RID: 147
		// (get) Token: 0x06000368 RID: 872 RVA: 0x0000E3DE File Offset: 0x0000C5DE
		// (set) Token: 0x06000369 RID: 873 RVA: 0x0000E3E6 File Offset: 0x0000C5E6
		public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings();

		// Token: 0x0600036A RID: 874 RVA: 0x0000E3F0 File Offset: 0x0000C5F0
		public DynamicConfigFile(string filename) : base(filename)
		{
			this._keyvalues = new Dictionary<string, object>();
			this._settings = new JsonSerializerSettings();
			this._settings.Converters.Add(new KeyValuesConverter());
			this._chroot = Interface.Oxide.InstanceDirectory;
		}

		// Token: 0x0600036B RID: 875 RVA: 0x0000E44C File Offset: 0x0000C64C
		public override void Load(string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string value = File.ReadAllText(filename);
			this._keyvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(value, this._settings);
		}

		// Token: 0x0600036C RID: 876 RVA: 0x0000E488 File Offset: 0x0000C688
		public T ReadObject<T>(string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			T t;
			if (this.Exists(filename))
			{
				t = JsonConvert.DeserializeObject<T>(File.ReadAllText(filename), this.Settings);
			}
			else
			{
				t = Activator.CreateInstance<T>();
				this.WriteObject<T>(t, false, filename);
			}
			return t;
		}

		// Token: 0x0600036D RID: 877 RVA: 0x0000E4D8 File Offset: 0x0000C6D8
		public override void Save(string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string directoryName = Utility.GetDirectoryName(filename);
			if (directoryName != null && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(filename, JsonConvert.SerializeObject(this._keyvalues, Formatting.Indented, this._settings));
		}

		// Token: 0x0600036E RID: 878 RVA: 0x0000E52C File Offset: 0x0000C72C
		public void WriteObject<T>(T config, bool sync = false, string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string directoryName = Utility.GetDirectoryName(filename);
			if (directoryName != null && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			string text = JsonConvert.SerializeObject(config, Formatting.Indented, this.Settings);
			File.WriteAllText(filename, text);
			if (sync)
			{
				this._keyvalues = JsonConvert.DeserializeObject<Dictionary<string, object>>(text, this._settings);
			}
		}

		// Token: 0x0600036F RID: 879 RVA: 0x0000E594 File Offset: 0x0000C794
		public bool Exists(string filename = null)
		{
			filename = this.CheckPath(filename ?? base.Filename);
			string directoryName = Utility.GetDirectoryName(filename);
			return (directoryName == null || Directory.Exists(directoryName)) && File.Exists(filename);
		}

		// Token: 0x06000370 RID: 880 RVA: 0x0000E5D0 File Offset: 0x0000C7D0
		private string CheckPath(string filename)
		{
			filename = DynamicConfigFile.SanitizeName(filename);
			string fullPath = Path.GetFullPath(filename);
			if (!fullPath.StartsWith(this._chroot, StringComparison.Ordinal))
			{
				throw new Exception("Only access to oxide directory!\nPath: " + fullPath);
			}
			return fullPath;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x0000E610 File Offset: 0x0000C810
		public static string SanitizeName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return string.Empty;
			}
			name = name.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
			name = Regex.Replace(name, "[" + Regex.Escape(new string(Path.GetInvalidPathChars())) + "]", "_");
			name = Regex.Replace(name, "\\.+", ".");
			return name.TrimStart(new char[]
			{
				'.'
			});
		}

		// Token: 0x06000372 RID: 882 RVA: 0x0000E694 File Offset: 0x0000C894
		[Obsolete("SanitiseName is deprecated, use SanitizeName instead")]
		public static string SanitiseName(string name)
		{
			return DynamicConfigFile.SanitizeName(name);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x0000E69C File Offset: 0x0000C89C
		public void Clear()
		{
			this._keyvalues.Clear();
		}

		// Token: 0x06000374 RID: 884 RVA: 0x0000E6A9 File Offset: 0x0000C8A9
		public void Remove(string key)
		{
			this._keyvalues.Remove(key);
		}

		// Token: 0x17000094 RID: 148
		public object this[string key]
		{
			get
			{
				object result;
				if (!this._keyvalues.TryGetValue(key, out result))
				{
					return null;
				}
				return result;
			}
			set
			{
				this._keyvalues[key] = value;
			}
		}

		// Token: 0x17000095 RID: 149
		public object this[string keyLevel1, string keyLevel2]
		{
			get
			{
				return this.Get(new string[]
				{
					keyLevel1,
					keyLevel2
				});
			}
			set
			{
				this.Set(new object[]
				{
					keyLevel1,
					keyLevel2,
					value
				});
			}
		}

		// Token: 0x17000096 RID: 150
		public object this[string keyLevel1, string keyLevel2, string keyLevel3]
		{
			get
			{
				return this.Get(new string[]
				{
					keyLevel1,
					keyLevel2,
					keyLevel3
				});
			}
			set
			{
				this.Set(new object[]
				{
					keyLevel1,
					keyLevel2,
					keyLevel3,
					value
				});
			}
		}

		// Token: 0x0600037B RID: 891 RVA: 0x0000E750 File Offset: 0x0000C950
		public object ConvertValue(object value, Type destinationType)
		{
			if (!destinationType.IsGenericType)
			{
				return Convert.ChangeType(value, destinationType);
			}
			if (destinationType.GetGenericTypeDefinition() == typeof(List<>))
			{
				Type conversionType = destinationType.GetGenericArguments()[0];
				IList list = (IList)Activator.CreateInstance(destinationType);
				foreach (object value2 in ((IList)value))
				{
					list.Add(Convert.ChangeType(value2, conversionType));
				}
				return list;
			}
			if (destinationType.GetGenericTypeDefinition() == typeof(Dictionary<, >))
			{
				Type conversionType2 = destinationType.GetGenericArguments()[0];
				Type conversionType3 = destinationType.GetGenericArguments()[1];
				IDictionary dictionary = (IDictionary)Activator.CreateInstance(destinationType);
				foreach (object obj in ((IDictionary)value).Keys)
				{
					dictionary.Add(Convert.ChangeType(obj, conversionType2), Convert.ChangeType(((IDictionary)value)[obj], conversionType3));
				}
				return dictionary;
			}
			throw new InvalidCastException("Generic types other than List<> and Dictionary<,> are not supported");
		}

		// Token: 0x0600037C RID: 892 RVA: 0x0000E894 File Offset: 0x0000CA94
		public T ConvertValue<T>(object value)
		{
			return (T)((object)this.ConvertValue(value, typeof(T)));
		}

		// Token: 0x0600037D RID: 893 RVA: 0x0000E8AC File Offset: 0x0000CAAC
		public object Get(params string[] path)
		{
			if (path.Length < 1)
			{
				throw new ArgumentException("path must not be empty");
			}
			object obj;
			if (!this._keyvalues.TryGetValue(path[0], out obj))
			{
				return null;
			}
			for (int i = 1; i < path.Length; i++)
			{
				Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
				if (dictionary == null || !dictionary.TryGetValue(path[i], out obj))
				{
					return null;
				}
			}
			return obj;
		}

		// Token: 0x0600037E RID: 894 RVA: 0x0000E906 File Offset: 0x0000CB06
		public T Get<T>(params string[] path)
		{
			return this.ConvertValue<T>(this.Get(path));
		}

		// Token: 0x0600037F RID: 895 RVA: 0x0000E918 File Offset: 0x0000CB18
		public void Set(params object[] pathAndTrailingValue)
		{
			if (pathAndTrailingValue.Length < 2)
			{
				throw new ArgumentException("path must not be empty");
			}
			string[] array = new string[pathAndTrailingValue.Length - 1];
			for (int i = 0; i < pathAndTrailingValue.Length - 1; i++)
			{
				array[i] = (string)pathAndTrailingValue[i];
			}
			object value = pathAndTrailingValue[pathAndTrailingValue.Length - 1];
			if (array.Length == 1)
			{
				this._keyvalues[array[0]] = value;
				return;
			}
			object obj;
			if (!this._keyvalues.TryGetValue(array[0], out obj))
			{
				obj = (this._keyvalues[array[0]] = new Dictionary<string, object>());
			}
			for (int j = 1; j < array.Length - 1; j++)
			{
				if (!(obj is Dictionary<string, object>))
				{
					throw new ArgumentException("path is not a dictionary");
				}
				Dictionary<string, object> dictionary = (Dictionary<string, object>)obj;
				if (!dictionary.TryGetValue(array[j], out obj))
				{
					obj = (dictionary[array[j]] = new Dictionary<string, object>());
				}
			}
			((Dictionary<string, object>)obj)[array[array.Length - 1]] = value;
		}

		// Token: 0x06000380 RID: 896 RVA: 0x0000EA04 File Offset: 0x0000CC04
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this._keyvalues.GetEnumerator();
		}

		// Token: 0x06000381 RID: 897 RVA: 0x0000EA16 File Offset: 0x0000CC16
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._keyvalues.GetEnumerator();
		}

		// Token: 0x04000140 RID: 320
		private Dictionary<string, object> _keyvalues;

		// Token: 0x04000141 RID: 321
		private readonly JsonSerializerSettings _settings;

		// Token: 0x04000142 RID: 322
		private readonly string _chroot;
	}
}
