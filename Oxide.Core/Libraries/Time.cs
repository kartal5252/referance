using System;

namespace Oxide.Core.Libraries
{
	// Token: 0x0200003E RID: 62
	public class Time : Library
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000265 RID: 613 RVA: 0x0000BDDE File Offset: 0x00009FDE
		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000266 RID: 614 RVA: 0x0000BDE1 File Offset: 0x00009FE1
		[LibraryFunction("GetCurrentTime")]
		public DateTime GetCurrentTime()
		{
			return DateTime.UtcNow;
		}

		// Token: 0x06000267 RID: 615 RVA: 0x0000BDE8 File Offset: 0x00009FE8
		[LibraryFunction("GetDateTimeFromUnix")]
		public DateTime GetDateTimeFromUnix(uint timestamp)
		{
			return Time.Epoch.AddSeconds(timestamp);
		}

		// Token: 0x06000268 RID: 616 RVA: 0x0000BE08 File Offset: 0x0000A008
		[LibraryFunction("GetUnixTimestamp")]
		public uint GetUnixTimestamp()
		{
			return (uint)DateTime.UtcNow.Subtract(Time.Epoch).TotalSeconds;
		}

		// Token: 0x06000269 RID: 617 RVA: 0x0000BE30 File Offset: 0x0000A030
		[LibraryFunction("GetUnixFromDateTime")]
		public uint GetUnixFromDateTime(DateTime time)
		{
			return (uint)time.Subtract(Time.Epoch).TotalSeconds;
		}

		// Token: 0x040000E9 RID: 233
		private static readonly DateTime Epoch = new DateTime(1970, 1, 1);
	}
}
