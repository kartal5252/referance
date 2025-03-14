using System;
using System.IO;

namespace Oxide.Core.Logging
{
	// Token: 0x02000033 RID: 51
	public sealed class RotatingFileLogger : ThreadedLogger
	{
		// Token: 0x17000040 RID: 64
		// (get) Token: 0x060001E8 RID: 488 RVA: 0x00009F0D File Offset: 0x0000810D
		// (set) Token: 0x060001E9 RID: 489 RVA: 0x00009F15 File Offset: 0x00008115
		public string Directory { get; set; }

		// Token: 0x060001EA RID: 490 RVA: 0x00009F1E File Offset: 0x0000811E
		private string GetLogFilename(DateTime date)
		{
			return Path.Combine(this.Directory, string.Format("oxide_{0:yyyy-MM-dd}.txt", date));
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00009F3B File Offset: 0x0000813B
		protected override void BeginBatchProcess()
		{
			this.writer = new StreamWriter(new FileStream(this.GetLogFilename(DateTime.Now), FileMode.Append, FileAccess.Write));
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00009F5A File Offset: 0x0000815A
		protected override void ProcessMessage(Logger.LogMessage message)
		{
			this.writer.WriteLine(message.LogfileMessage);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00009F6D File Offset: 0x0000816D
		protected override void FinishBatchProcess()
		{
			this.writer.Close();
			this.writer.Dispose();
			this.writer = null;
		}

		// Token: 0x040000CD RID: 205
		private StreamWriter writer;
	}
}
