using System;
using System.IO;
using Newtonsoft.Json;

namespace Oxide.Core.Configuration
{
	// Token: 0x02000057 RID: 87
	public abstract class ConfigFile
	{
		// Token: 0x17000092 RID: 146
		// (get) Token: 0x06000362 RID: 866 RVA: 0x0000E351 File Offset: 0x0000C551
		// (set) Token: 0x06000363 RID: 867 RVA: 0x0000E359 File Offset: 0x0000C559
		[JsonIgnore]
		public string Filename { get; private set; }

		// Token: 0x06000364 RID: 868 RVA: 0x0000E362 File Offset: 0x0000C562
		protected ConfigFile(string filename)
		{
			this.Filename = filename;
		}

		// Token: 0x06000365 RID: 869 RVA: 0x0000E371 File Offset: 0x0000C571
		public static T Load<T>(string filename) where T : ConfigFile
		{
			T t = (T)((object)Activator.CreateInstance(typeof(T), new object[]
			{
				filename
			}));
			t.Load(null);
			return t;
		}

		// Token: 0x06000366 RID: 870 RVA: 0x0000E39D File Offset: 0x0000C59D
		public virtual void Load(string filename = null)
		{
			JsonConvert.PopulateObject(File.ReadAllText(filename ?? this.Filename), this);
		}

		// Token: 0x06000367 RID: 871 RVA: 0x0000E3B8 File Offset: 0x0000C5B8
		public virtual void Save(string filename = null)
		{
			string contents = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(filename ?? this.Filename, contents);
		}
	}
}
