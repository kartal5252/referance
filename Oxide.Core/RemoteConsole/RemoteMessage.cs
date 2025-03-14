using System;
using Newtonsoft.Json;

namespace Oxide.Core.RemoteConsole
{
	// Token: 0x0200001E RID: 30
	[Serializable]
	public class RemoteMessage
	{
		// Token: 0x0600013C RID: 316 RVA: 0x00007A64 File Offset: 0x00005C64
		public static RemoteMessage CreateMessage(string message, int identifier = -1, string type = "Generic", string trace = "")
		{
			return new RemoteMessage
			{
				Message = message,
				Identifier = identifier,
				Type = type,
				Stacktrace = trace
			};
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00007A88 File Offset: 0x00005C88
		public static RemoteMessage GetMessage(string text)
		{
			RemoteMessage result;
			try
			{
				result = JsonConvert.DeserializeObject<RemoteMessage>(text);
			}
			catch (JsonReaderException)
			{
				Interface.Oxide.LogError("[Rcon] Failed to parse message, incorrect format", new object[0]);
				result = null;
			}
			return result;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00007ACC File Offset: 0x00005CCC
		internal string ToJSON()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		// Token: 0x04000083 RID: 131
		public string Message;

		// Token: 0x04000084 RID: 132
		public int Identifier;

		// Token: 0x04000085 RID: 133
		public string Type;

		// Token: 0x04000086 RID: 134
		public string Stacktrace;
	}
}
