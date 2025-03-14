using System;
using System.Linq;

namespace Oxide.Core.ServerConsole
{
	// Token: 0x0200001C RID: 28
	public class ServerConsole
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000113 RID: 275 RVA: 0x00007120 File Offset: 0x00005320
		// (remove) Token: 0x06000114 RID: 276 RVA: 0x00007158 File Offset: 0x00005358
		public event Action<string> Input;

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000115 RID: 277 RVA: 0x0000718D File Offset: 0x0000538D
		// (set) Token: 0x06000116 RID: 278 RVA: 0x0000719A File Offset: 0x0000539A
		public Func<string, string[]> Completion
		{
			get
			{
				return this.input.Completion;
			}
			set
			{
				this.input.Completion = value;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000117 RID: 279 RVA: 0x000071A8 File Offset: 0x000053A8
		// (set) Token: 0x06000118 RID: 280 RVA: 0x000071B7 File Offset: 0x000053B7
		public ConsoleColor Status1LeftColor
		{
			get
			{
				return this.input.StatusTextLeftColor[1];
			}
			set
			{
				this.input.StatusTextLeftColor[1] = value;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000119 RID: 281 RVA: 0x000071C7 File Offset: 0x000053C7
		// (set) Token: 0x0600011A RID: 282 RVA: 0x000071D6 File Offset: 0x000053D6
		public ConsoleColor Status1RightColor
		{
			get
			{
				return this.input.StatusTextRightColor[1];
			}
			set
			{
				this.input.StatusTextRightColor[1] = value;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600011B RID: 283 RVA: 0x000071E6 File Offset: 0x000053E6
		// (set) Token: 0x0600011C RID: 284 RVA: 0x000071F5 File Offset: 0x000053F5
		public ConsoleColor Status2LeftColor
		{
			get
			{
				return this.input.StatusTextLeftColor[2];
			}
			set
			{
				this.input.StatusTextLeftColor[2] = value;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600011D RID: 285 RVA: 0x00007205 File Offset: 0x00005405
		// (set) Token: 0x0600011E RID: 286 RVA: 0x00007214 File Offset: 0x00005414
		public ConsoleColor Status2RightColor
		{
			get
			{
				return this.input.StatusTextRightColor[2];
			}
			set
			{
				this.input.StatusTextRightColor[2] = value;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00007224 File Offset: 0x00005424
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00007233 File Offset: 0x00005433
		public ConsoleColor Status3RightColor
		{
			get
			{
				return this.input.StatusTextRightColor[3];
			}
			set
			{
				this.input.StatusTextRightColor[3] = value;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000121 RID: 289 RVA: 0x00007243 File Offset: 0x00005443
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00007252 File Offset: 0x00005452
		public ConsoleColor Status3LeftColor
		{
			get
			{
				return this.input.StatusTextLeftColor[3];
			}
			set
			{
				this.input.StatusTextLeftColor[3] = value;
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000123 RID: 291 RVA: 0x00007262 File Offset: 0x00005462
		private string title
		{
			get
			{
				Func<string> title = this.Title;
				if (title == null)
				{
					return null;
				}
				return title();
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00007275 File Offset: 0x00005475
		private string status1Left
		{
			get
			{
				return ServerConsole.GetStatusValue(this.Status1Left);
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000125 RID: 293 RVA: 0x00007282 File Offset: 0x00005482
		private string status1Right
		{
			get
			{
				return ServerConsole.GetStatusValue(this.Status1Right).PadLeft(this.input.LineWidth - 1);
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x06000126 RID: 294 RVA: 0x000072A1 File Offset: 0x000054A1
		private string status2Left
		{
			get
			{
				return ServerConsole.GetStatusValue(this.Status2Left);
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x06000127 RID: 295 RVA: 0x000072AE File Offset: 0x000054AE
		private string status2Right
		{
			get
			{
				return ServerConsole.GetStatusValue(this.Status2Right).PadLeft(this.input.LineWidth - 1);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000128 RID: 296 RVA: 0x000072CD File Offset: 0x000054CD
		private string status3Left
		{
			get
			{
				return ServerConsole.GetStatusValue(this.Status3Left);
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000129 RID: 297 RVA: 0x000072DA File Offset: 0x000054DA
		private string status3Right
		{
			get
			{
				return ServerConsole.GetStatusValue(this.Status3Right).PadLeft(this.input.LineWidth - 1);
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x000072F9 File Offset: 0x000054F9
		private static string GetStatusValue(Func<string> status)
		{
			if (status == null)
			{
				return "";
			}
			return status() ?? string.Empty;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00007313 File Offset: 0x00005513
		private static string GetStatusRight(int leftLength, string right)
		{
			if (leftLength < right.Length)
			{
				return right.Substring(leftLength);
			}
			return string.Empty;
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000732C File Offset: 0x0000552C
		public void AddMessage(string message, ConsoleColor color = ConsoleColor.Gray)
		{
			Console.ForegroundColor = color;
			int num = message.Split(new char[]
			{
				'\n'
			}).Aggregate(0, (int sum, string line) => sum + (int)Math.Ceiling((double)line.Length / (double)Console.BufferWidth));
			this.input.ClearLine((Interface.Oxide.Config.Console.ShowStatusBar ? this.input.StatusTextLeft.Length : 0) + num);
			Console.WriteLine(message);
			this.input.RedrawInputLine();
			Console.ForegroundColor = ConsoleColor.Gray;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x000073C0 File Offset: 0x000055C0
		public void OnDisable()
		{
			if (!this.init)
			{
				return;
			}
			this.input.OnInputText -= this.OnInputText;
			this.console.Shutdown();
		}

		// Token: 0x0600012E RID: 302 RVA: 0x000073F0 File Offset: 0x000055F0
		public void OnEnable()
		{
			if (!this.console.Initialize())
			{
				return;
			}
			this.init = true;
			this.input.OnInputText += this.OnInputText;
			this.input.ClearLine(1);
			this.input.ClearLine(Console.WindowHeight);
			for (int i = 0; i < Console.WindowHeight; i++)
			{
				Console.WriteLine();
			}
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000745C File Offset: 0x0000565C
		private void OnInputText(string obj)
		{
			try
			{
				Action<string> action = this.Input;
				if (action != null)
				{
					action(obj);
				}
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException("OnInputText: ", ex);
			}
		}

		// Token: 0x06000130 RID: 304 RVA: 0x000074A0 File Offset: 0x000056A0
		public static void PrintColored(params object[] objects)
		{
			if (Interface.Oxide.ServerConsole == null)
			{
				return;
			}
			Interface.Oxide.ServerConsole.input.ClearLine(Interface.Oxide.Config.Console.ShowStatusBar ? Interface.Oxide.ServerConsole.input.StatusTextLeft.Length : 1);
			for (int i = 0; i < objects.Length; i++)
			{
				if (i % 2 != 0)
				{
					Console.Write((string)objects[i]);
				}
				else
				{
					Console.ForegroundColor = (ConsoleColor)((int)objects[i]);
				}
			}
			if (Console.CursorLeft != 0)
			{
				Console.CursorTop++;
			}
			Interface.Oxide.ServerConsole.input.RedrawInputLine();
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00007554 File Offset: 0x00005754
		public void Update()
		{
			if (!this.init)
			{
				return;
			}
			if (Interface.Oxide.Config.Console.ShowStatusBar)
			{
				this.UpdateStatus();
			}
			this.input.Update();
			if (this.nextTitleUpdate > Interface.Oxide.Now)
			{
				return;
			}
			this.nextTitleUpdate = Interface.Oxide.Now + 1f;
			this.console.SetTitle(this.title);
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000075CC File Offset: 0x000057CC
		private void UpdateStatus()
		{
			if (this.nextUpdate > Interface.Oxide.Now)
			{
				return;
			}
			this.nextUpdate = Interface.Oxide.Now + 0.66f;
			if (!this.input.Valid)
			{
				return;
			}
			string status1Left = this.status1Left;
			string status2Left = this.status2Left;
			string status3Left = this.status3Left;
			this.input.StatusTextLeft[1] = status1Left;
			this.input.StatusTextLeft[2] = status2Left;
			this.input.StatusTextLeft[3] = status3Left;
			this.input.StatusTextRight[1] = ServerConsole.GetStatusRight(status1Left.Length, this.status1Right);
			this.input.StatusTextRight[2] = ServerConsole.GetStatusRight(status2Left.Length, this.status2Right);
			this.input.StatusTextRight[3] = ServerConsole.GetStatusRight(status3Left.Length, this.status3Right);
		}

		// Token: 0x04000072 RID: 114
		private readonly ConsoleWindow console = new ConsoleWindow();

		// Token: 0x04000073 RID: 115
		private readonly ConsoleInput input = new ConsoleInput();

		// Token: 0x04000074 RID: 116
		private bool init;

		// Token: 0x04000075 RID: 117
		private float nextUpdate;

		// Token: 0x04000076 RID: 118
		private float nextTitleUpdate;

		// Token: 0x04000078 RID: 120
		public Func<string> Title;

		// Token: 0x04000079 RID: 121
		public Func<string> Status1Left;

		// Token: 0x0400007A RID: 122
		public Func<string> Status1Right;

		// Token: 0x0400007B RID: 123
		public Func<string> Status2Left;

		// Token: 0x0400007C RID: 124
		public Func<string> Status2Right;

		// Token: 0x0400007D RID: 125
		public Func<string> Status3Left;

		// Token: 0x0400007E RID: 126
		public Func<string> Status3Right;
	}
}
