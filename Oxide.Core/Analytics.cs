using System;
using System.Collections.Generic;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;

namespace Oxide.Core
{
	// Token: 0x02000002 RID: 2
	public static class Analytics
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static void Collect()
		{
			Analytics.SendPayload(string.Concat(new string[]
			{
				"v=1&tid=UA-48448359-3&cid=",
				Analytics.Identifier,
				"&t=screenview&cd=",
				Analytics.Covalence.Game,
				"+",
				Analytics.Covalence.Server.Version
			}) + string.Format("&an=Oxide&av={0}&ul={1}", OxideMod.Version, Analytics.Lang.GetServerLanguage()));
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020CF File Offset: 0x000002CF
		public static void Event(string category, string action)
		{
			Analytics.SendPayload(string.Concat(new string[]
			{
				"v=1&tid=UA-48448359-3&cid=",
				Analytics.Identifier,
				"&t=event&ec=",
				category,
				"&ea=",
				action
			}));
		}

		// Token: 0x06000003 RID: 3 RVA: 0x0000210C File Offset: 0x0000030C
		public static void SendPayload(string payload)
		{
			Dictionary<string, string> headers = new Dictionary<string, string>
			{
				{
					"User-Agent",
					string.Format("Oxide/{0} ({1}; {2})", OxideMod.Version, Environment.OSVersion, Environment.OSVersion.Platform)
				}
			};
			Analytics.Webrequests.Enqueue("https://www.google-analytics.com/collect", Uri.EscapeUriString(payload), delegate(int code, string response)
			{
			}, null, RequestMethod.POST, headers, 0f);
		}

		// Token: 0x04000001 RID: 1
		private static readonly WebRequests Webrequests = Interface.Oxide.GetLibrary<WebRequests>(null);

		// Token: 0x04000002 RID: 2
		private static readonly PluginManager PluginManager = Interface.Oxide.RootPluginManager;

		// Token: 0x04000003 RID: 3
		private static readonly Covalence Covalence = Interface.Oxide.GetLibrary<Covalence>(null);

		// Token: 0x04000004 RID: 4
		private static readonly Lang Lang = Interface.Oxide.GetLibrary<Lang>(null);

		// Token: 0x04000005 RID: 5
		private const string trackingId = "UA-48448359-3";

		// Token: 0x04000006 RID: 6
		private const string url = "https://www.google-analytics.com/collect";

		// Token: 0x04000007 RID: 7
		private static readonly string Identifier = string.Format("{0}:{1}", Analytics.Covalence.Server.Address, Analytics.Covalence.Server.Port);
	}
}
