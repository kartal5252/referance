using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Core.Configuration
{
	// Token: 0x0200005A RID: 90
	public class OxideConfig : ConfigFile
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x06000387 RID: 903 RVA: 0x0000EDC8 File Offset: 0x0000CFC8
		// (set) Token: 0x06000388 RID: 904 RVA: 0x0000EDD0 File Offset: 0x0000CFD0
		public OxideConfig.OxideOptions Options { get; set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x06000389 RID: 905 RVA: 0x0000EDD9 File Offset: 0x0000CFD9
		// (set) Token: 0x0600038A RID: 906 RVA: 0x0000EDE1 File Offset: 0x0000CFE1
		[JsonProperty(PropertyName = "OxideConsole")]
		public OxideConfig.OxideConsole Console { get; set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x0600038B RID: 907 RVA: 0x0000EDEA File Offset: 0x0000CFEA
		// (set) Token: 0x0600038C RID: 908 RVA: 0x0000EDF2 File Offset: 0x0000CFF2
		[JsonProperty(PropertyName = "OxideRcon")]
		public OxideConfig.OxideRcon Rcon { get; set; }

		// Token: 0x0600038D RID: 909 RVA: 0x0000EDFC File Offset: 0x0000CFFC
		public OxideConfig(string filename) : base(filename)
		{
			this.Options = new OxideConfig.OxideOptions
			{
				Modded = true,
				PluginWatchers = true,
				DefaultGroups = new OxideConfig.DefaultGroups
				{
					Administrators = "admin",
					Players = "default"
				}
			};
			this.Console = new OxideConfig.OxideConsole
			{
				Enabled = true,
				MinimalistMode = true,
				ShowStatusBar = true
			};
			this.Rcon = new OxideConfig.OxideRcon
			{
				Enabled = false,
				ChatPrefix = "[Server Console]",
				Port = 25580,
				Password = string.Empty
			};
		}

		// Token: 0x020000A6 RID: 166
		public class OxideOptions
		{
			// Token: 0x0400022A RID: 554
			public bool Modded;

			// Token: 0x0400022B RID: 555
			public bool PluginWatchers;

			// Token: 0x0400022C RID: 556
			public OxideConfig.DefaultGroups DefaultGroups;
		}

		// Token: 0x020000A7 RID: 167
		[JsonObject]
		public class DefaultGroups : IEnumerable<string>, IEnumerable
		{
			// Token: 0x0600049C RID: 1180 RVA: 0x000117DD File Offset: 0x0000F9DD
			public IEnumerator<string> GetEnumerator()
			{
				yield return this.Players;
				yield return this.Administrators;
				yield break;
			}

			// Token: 0x0600049D RID: 1181 RVA: 0x000117EC File Offset: 0x0000F9EC
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			// Token: 0x0400022D RID: 557
			public string Players;

			// Token: 0x0400022E RID: 558
			public string Administrators;
		}

		// Token: 0x020000A8 RID: 168
		public class OxideConsole
		{
			// Token: 0x170000B6 RID: 182
			// (get) Token: 0x0600049F RID: 1183 RVA: 0x000117FC File Offset: 0x0000F9FC
			// (set) Token: 0x060004A0 RID: 1184 RVA: 0x00011804 File Offset: 0x0000FA04
			public bool Enabled { get; set; }

			// Token: 0x170000B7 RID: 183
			// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001180D File Offset: 0x0000FA0D
			// (set) Token: 0x060004A2 RID: 1186 RVA: 0x00011815 File Offset: 0x0000FA15
			public bool MinimalistMode { get; set; }

			// Token: 0x170000B8 RID: 184
			// (get) Token: 0x060004A3 RID: 1187 RVA: 0x0001181E File Offset: 0x0000FA1E
			// (set) Token: 0x060004A4 RID: 1188 RVA: 0x00011826 File Offset: 0x0000FA26
			public bool ShowStatusBar { get; set; }
		}

		// Token: 0x020000A9 RID: 169
		public class OxideRcon
		{
			// Token: 0x170000B9 RID: 185
			// (get) Token: 0x060004A6 RID: 1190 RVA: 0x00011837 File Offset: 0x0000FA37
			// (set) Token: 0x060004A7 RID: 1191 RVA: 0x0001183F File Offset: 0x0000FA3F
			public bool Enabled { get; set; }

			// Token: 0x170000BA RID: 186
			// (get) Token: 0x060004A8 RID: 1192 RVA: 0x00011848 File Offset: 0x0000FA48
			// (set) Token: 0x060004A9 RID: 1193 RVA: 0x00011850 File Offset: 0x0000FA50
			public int Port { get; set; }

			// Token: 0x170000BB RID: 187
			// (get) Token: 0x060004AA RID: 1194 RVA: 0x00011859 File Offset: 0x0000FA59
			// (set) Token: 0x060004AB RID: 1195 RVA: 0x00011861 File Offset: 0x0000FA61
			public string Password { get; set; }

			// Token: 0x170000BC RID: 188
			// (get) Token: 0x060004AC RID: 1196 RVA: 0x0001186A File Offset: 0x0000FA6A
			// (set) Token: 0x060004AD RID: 1197 RVA: 0x00011872 File Offset: 0x0000FA72
			public string ChatPrefix { get; set; }
		}
	}
}
