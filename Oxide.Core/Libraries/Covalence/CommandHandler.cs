using System;
using System.Collections.Generic;
using System.Text;

namespace Oxide.Core.Libraries.Covalence
{
	// Token: 0x02000043 RID: 67
	public sealed class CommandHandler
	{
		// Token: 0x06000284 RID: 644 RVA: 0x0000C58E File Offset: 0x0000A78E
		public CommandHandler(CommandCallback callback, Func<string, bool> commandFilter)
		{
			this.callback = callback;
			this.commandFilter = commandFilter;
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000C5A4 File Offset: 0x0000A7A4
		public bool HandleChatMessage(IPlayer player, string message)
		{
			if (message.Length == 0)
			{
				return false;
			}
			if (message[0] != '/')
			{
				return false;
			}
			message = message.Substring(1);
			string text;
			string[] args;
			this.ParseCommand(message, out text, out args);
			player.LastCommand = CommandType.Chat;
			return text != null && this.HandleCommand(player, text, args);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000C5F4 File Offset: 0x0000A7F4
		public bool HandleConsoleMessage(IPlayer player, string message)
		{
			if (message.StartsWith("global."))
			{
				message = message.Substring(7);
			}
			string text;
			string[] args;
			this.ParseCommand(message, out text, out args);
			player.LastCommand = CommandType.Console;
			return text != null && this.HandleCommand(player, text, args);
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000C637 File Offset: 0x0000A837
		private bool HandleCommand(IPlayer player, string command, string[] args)
		{
			return (this.commandFilter == null || this.commandFilter(command)) && this.callback != null && this.callback(player, command, args);
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000C668 File Offset: 0x0000A868
		private void ParseCommand(string argstr, out string cmd, out string[] args)
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in argstr)
			{
				if (c == '"')
				{
					if (flag)
					{
						string text = stringBuilder.ToString().Trim();
						if (!string.IsNullOrEmpty(text))
						{
							list.Add(text);
						}
						stringBuilder = new StringBuilder();
						flag = false;
					}
					else
					{
						flag = true;
					}
				}
				else if (char.IsWhiteSpace(c) && !flag)
				{
					string text2 = stringBuilder.ToString().Trim();
					if (!string.IsNullOrEmpty(text2))
					{
						list.Add(text2);
					}
					stringBuilder = new StringBuilder();
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			if (stringBuilder.Length > 0)
			{
				string text3 = stringBuilder.ToString().Trim();
				if (!string.IsNullOrEmpty(text3))
				{
					list.Add(text3);
				}
			}
			if (list.Count == 0)
			{
				cmd = null;
				args = null;
				return;
			}
			cmd = list[0].ToLowerInvariant();
			list.RemoveAt(0);
			args = list.ToArray();
		}

		// Token: 0x04000106 RID: 262
		private CommandCallback callback;

		// Token: 0x04000107 RID: 263
		private Func<string, bool> commandFilter;
	}
}
