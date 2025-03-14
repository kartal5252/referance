using System;
using System.Collections.Generic;
using System.IO;

namespace Oxide.Core
{
	// Token: 0x02000004 RID: 4
	public static class Cleanup
	{
		// Token: 0x06000009 RID: 9 RVA: 0x00002370 File Offset: 0x00000570
		public static void Add(string file)
		{
			Cleanup.files.Add(file);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002380 File Offset: 0x00000580
		internal static void Run()
		{
			if (Cleanup.files == null)
			{
				return;
			}
			foreach (string text in Cleanup.files)
			{
				try
				{
					if (File.Exists(text))
					{
						Interface.Oxide.LogDebug("Cleanup file: {0}", new object[]
						{
							text
						});
						File.Delete(text);
					}
				}
				catch (Exception)
				{
					Interface.Oxide.LogWarning("Failed to cleanup file: {0}", new object[]
					{
						text
					});
				}
			}
			Cleanup.files = null;
		}

		// Token: 0x0400000C RID: 12
		internal static HashSet<string> files = new HashSet<string>();
	}
}
