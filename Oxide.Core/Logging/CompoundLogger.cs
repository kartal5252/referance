using System;
using System.Collections.Generic;

namespace Oxide.Core.Logging
{
	// Token: 0x02000030 RID: 48
	public sealed class CompoundLogger : Logger
	{
		// Token: 0x060001DA RID: 474 RVA: 0x00009B0E File Offset: 0x00007D0E
		public CompoundLogger() : base(true)
		{
			this.subloggers = new HashSet<Logger>();
			this.messagecache = new List<Logger.LogMessage>();
			this.usecache = true;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00009B34 File Offset: 0x00007D34
		public void AddLogger(Logger logger)
		{
			this.subloggers.Add(logger);
			foreach (Logger.LogMessage message in this.messagecache)
			{
				logger.Write(message);
			}
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00009B94 File Offset: 0x00007D94
		public void RemoveLogger(Logger logger)
		{
			logger.OnRemoved();
			this.subloggers.Remove(logger);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00009BAC File Offset: 0x00007DAC
		public void Shutdown()
		{
			foreach (Logger logger in this.subloggers)
			{
				logger.OnRemoved();
			}
			this.subloggers.Clear();
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00009C08 File Offset: 0x00007E08
		public override void Write(LogType type, string format, params object[] args)
		{
			foreach (Logger logger in this.subloggers)
			{
				logger.Write(type, format, args);
			}
			if (this.usecache)
			{
				this.messagecache.Add(base.CreateLogMessage(type, format, args));
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00009C78 File Offset: 0x00007E78
		public void DisableCache()
		{
			this.usecache = false;
			this.messagecache.Clear();
		}

		// Token: 0x040000C2 RID: 194
		private readonly HashSet<Logger> subloggers;

		// Token: 0x040000C3 RID: 195
		private readonly List<Logger.LogMessage> messagecache;

		// Token: 0x040000C4 RID: 196
		private bool usecache;
	}
}
