using System;

namespace Oxide.Core.Logging
{
	// Token: 0x0200002F RID: 47
	public class CallbackLogger : Logger
	{
		// Token: 0x060001D8 RID: 472 RVA: 0x00009AE6 File Offset: 0x00007CE6
		public CallbackLogger(NativeDebugCallback callback) : base(true)
		{
			this.callback = callback;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00009AF6 File Offset: 0x00007CF6
		protected override void ProcessMessage(Logger.LogMessage message)
		{
			NativeDebugCallback nativeDebugCallback = this.callback;
			if (nativeDebugCallback == null)
			{
				return;
			}
			nativeDebugCallback(message.LogfileMessage);
		}

		// Token: 0x040000C1 RID: 193
		private NativeDebugCallback callback;
	}
}
