using System;
using System.Threading;

namespace Oxide.Core.Logging
{
	// Token: 0x02000034 RID: 52
	public abstract class ThreadedLogger : Logger
	{
		// Token: 0x060001EF RID: 495 RVA: 0x00009F94 File Offset: 0x00008194
		public ThreadedLogger() : base(false)
		{
			this.waitevent = new AutoResetEvent(false);
			this.exit = false;
			this.syncroot = new object();
			this.workerthread = new Thread(new ThreadStart(this.Worker))
			{
				IsBackground = true
			};
			this.workerthread.Start();
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00009FF0 File Offset: 0x000081F0
		~ThreadedLogger()
		{
			this.OnRemoved();
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000A01C File Offset: 0x0000821C
		public override void OnRemoved()
		{
			if (this.exit)
			{
				return;
			}
			this.exit = true;
			this.waitevent.Set();
			this.workerthread.Join();
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x0000A048 File Offset: 0x00008248
		internal override void Write(Logger.LogMessage msg)
		{
			object obj = this.syncroot;
			lock (obj)
			{
				base.Write(msg);
			}
			this.waitevent.Set();
		}

		// Token: 0x060001F3 RID: 499
		protected abstract void BeginBatchProcess();

		// Token: 0x060001F4 RID: 500
		protected abstract void FinishBatchProcess();

		// Token: 0x060001F5 RID: 501 RVA: 0x0000A090 File Offset: 0x00008290
		private void Worker()
		{
			while (!this.exit)
			{
				this.waitevent.WaitOne();
				object obj = this.syncroot;
				lock (obj)
				{
					if (this.MessageQueue.Count > 0)
					{
						this.BeginBatchProcess();
						try
						{
							while (this.MessageQueue.Count > 0)
							{
								Logger.LogMessage message = this.MessageQueue.Dequeue();
								this.ProcessMessage(message);
							}
						}
						finally
						{
							this.FinishBatchProcess();
						}
					}
				}
			}
		}

		// Token: 0x040000CE RID: 206
		private AutoResetEvent waitevent;

		// Token: 0x040000CF RID: 207
		private bool exit;

		// Token: 0x040000D0 RID: 208
		private object syncroot;

		// Token: 0x040000D1 RID: 209
		private Thread workerthread;
	}
}
