using System;
using System.Collections.Generic;
using Oxide.Core.RemoteConsole;

namespace Oxide.Core.Logging
{
	// Token: 0x02000032 RID: 50
	public abstract class Logger
	{
		// Token: 0x060001E0 RID: 480 RVA: 0x00009C8C File Offset: 0x00007E8C
		protected Logger(bool processImediately)
		{
			this.processImediately = processImediately;
			if (!processImediately)
			{
				this.MessageQueue = new Queue<Logger.LogMessage>();
			}
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x00009CAC File Offset: 0x00007EAC
		protected Logger.LogMessage CreateLogMessage(LogType type, string format, object[] args)
		{
			Logger.LogMessage logMessage = new Logger.LogMessage
			{
				Type = type,
				ConsoleMessage = string.Format("[Oxide] {0} [{1}] {2}", DateTime.Now.ToShortTimeString(), type, format),
				LogfileMessage = string.Format("{0} [{1}] {2}", DateTime.Now.ToShortTimeString(), type, format)
			};
			if (Interface.Oxide.Config.Console.MinimalistMode)
			{
				logMessage.ConsoleMessage = format;
			}
			if (args.Length != 0)
			{
				logMessage.ConsoleMessage = string.Format(logMessage.ConsoleMessage, args);
				logMessage.LogfileMessage = string.Format(logMessage.LogfileMessage, args);
			}
			return logMessage;
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x00009D60 File Offset: 0x00007F60
		public virtual void HandleMessage(string message, string stackTrace, LogType logType)
		{
			if (message.ToLower().Contains("[chat]"))
			{
				logType = LogType.Chat;
			}
			ConsoleColor color;
			string type;
			switch (logType)
			{
			case LogType.Chat:
				color = ConsoleColor.Green;
				type = "Chat";
				goto IL_56;
			case LogType.Error:
				color = ConsoleColor.Red;
				type = "Error";
				goto IL_56;
			case LogType.Warning:
				color = ConsoleColor.Yellow;
				type = "Warning";
				goto IL_56;
			}
			color = ConsoleColor.Gray;
			type = "Generic";
			IL_56:
			Interface.Oxide.ServerConsole.AddMessage(message, color);
			Interface.Oxide.RemoteConsole.SendMessage(new RemoteMessage
			{
				Message = message,
				Identifier = -1,
				Type = type,
				Stacktrace = stackTrace
			});
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00009E04 File Offset: 0x00008004
		public virtual void Write(LogType type, string format, params object[] args)
		{
			Logger.LogMessage message = this.CreateLogMessage(type, format, args);
			this.Write(message);
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00009E22 File Offset: 0x00008022
		internal virtual void Write(Logger.LogMessage message)
		{
			if (this.processImediately)
			{
				this.ProcessMessage(message);
				return;
			}
			this.MessageQueue.Enqueue(message);
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00009E40 File Offset: 0x00008040
		protected virtual void ProcessMessage(Logger.LogMessage message)
		{
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00009E44 File Offset: 0x00008044
		public virtual void WriteException(string message, Exception ex)
		{
			string text = ExceptionHandler.FormatException(ex);
			if (text != null)
			{
				this.Write(LogType.Error, message + Environment.NewLine + text, new object[0]);
				return;
			}
			Exception ex2 = ex;
			while (ex.InnerException != null)
			{
				ex = ex.InnerException;
			}
			if (ex2.GetType() != ex.GetType())
			{
				this.Write(LogType.Error, "ExType: {0}", new object[]
				{
					ex2.GetType().Name
				});
			}
			this.Write(LogType.Error, string.Concat(new string[]
			{
				message,
				" (",
				ex.GetType().Name,
				": ",
				ex.Message,
				")\n",
				ex.StackTrace
			}), new object[0]);
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00009F0B File Offset: 0x0000810B
		public virtual void OnRemoved()
		{
		}

		// Token: 0x040000CA RID: 202
		protected Queue<Logger.LogMessage> MessageQueue;

		// Token: 0x040000CB RID: 203
		private bool processImediately;

		// Token: 0x02000083 RID: 131
		public struct LogMessage
		{
			// Token: 0x040001BD RID: 445
			public LogType Type;

			// Token: 0x040001BE RID: 446
			public string ConsoleMessage;

			// Token: 0x040001BF RID: 447
			public string LogfileMessage;
		}
	}
}
