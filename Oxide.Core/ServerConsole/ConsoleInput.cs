using System;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Core.ServerConsole
{
	// Token: 0x0200001A RID: 26
	public class ConsoleInput
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x060000FC RID: 252 RVA: 0x00006884 File Offset: 0x00004A84
		// (remove) Token: 0x060000FD RID: 253 RVA: 0x000068BC File Offset: 0x00004ABC
		internal event Action<string> OnInputText;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000FE RID: 254 RVA: 0x000068F1 File Offset: 0x00004AF1
		public int LineWidth
		{
			get
			{
				return Console.BufferWidth;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000FF RID: 255 RVA: 0x000068F8 File Offset: 0x00004AF8
		public bool Valid
		{
			get
			{
				return Console.BufferWidth > 0;
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006902 File Offset: 0x00004B02
		public void ClearLine(int numLines)
		{
			Console.CursorLeft = 0;
			Console.Write(new string(' ', this.LineWidth * numLines));
			Console.CursorTop -= numLines;
			Console.CursorLeft = 0;
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006930 File Offset: 0x00004B30
		public void RedrawInputLine()
		{
			if (this.nextUpdate - 0.45f > Interface.Oxide.Now || this.LineWidth <= 0)
			{
				return;
			}
			try
			{
				Console.CursorTop++;
				int num = 0;
				while (num < this.StatusTextLeft.Length && Interface.Oxide.Config.Console.ShowStatusBar)
				{
					Console.CursorLeft = 0;
					Console.ForegroundColor = this.StatusTextLeftColor[num];
					Console.Write(this.StatusTextLeft[num].Substring(0, Math.Min(this.StatusTextLeft[num].Length, this.LineWidth - 1)));
					Console.ForegroundColor = this.StatusTextRightColor[num];
					Console.Write(this.StatusTextRight[num].PadRight(this.LineWidth));
					num++;
				}
				Console.CursorTop -= (Interface.Oxide.Config.Console.ShowStatusBar ? (this.StatusTextLeft.Length + 1) : 1);
				Console.CursorLeft = 0;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Green;
				this.ClearLine(1);
				if (this.inputString.Length == 0)
				{
					Console.ForegroundColor = ConsoleColor.Gray;
				}
				else
				{
					Console.Write((this.inputString.Length >= this.LineWidth - 2) ? this.inputString.Substring(this.inputString.Length - (this.LineWidth - 2)) : this.inputString);
					Console.ForegroundColor = ConsoleColor.Gray;
				}
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException("RedrawInputLine: ", ex);
			}
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006AD4 File Offset: 0x00004CD4
		public void Update()
		{
			if (!this.Valid)
			{
				return;
			}
			if (this.nextUpdate < Interface.Oxide.Now)
			{
				this.RedrawInputLine();
				this.nextUpdate = Interface.Oxide.Now + 0.5f;
			}
			try
			{
				if (!Console.KeyAvailable)
				{
					return;
				}
			}
			catch (Exception)
			{
				return;
			}
			ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
			if (consoleKeyInfo.Key != ConsoleKey.DownArrow && consoleKeyInfo.Key != ConsoleKey.UpArrow)
			{
				this.inputHistoryIndex = 0;
			}
			ConsoleKey key = consoleKeyInfo.Key;
			if (key <= ConsoleKey.Enter)
			{
				if (key != ConsoleKey.Backspace)
				{
					if (key != ConsoleKey.Tab)
					{
						if (key == ConsoleKey.Enter)
						{
							this.ClearLine(Interface.Oxide.Config.Console.ShowStatusBar ? this.StatusTextLeft.Length : 1);
							Console.ForegroundColor = ConsoleColor.Green;
							Console.WriteLine("> " + this.inputString);
							this.inputHistory.Insert(0, this.inputString);
							if (this.inputHistory.Count > 50)
							{
								this.inputHistory.RemoveRange(50, this.inputHistory.Count - 50);
							}
							string obj = this.inputString;
							this.inputString = string.Empty;
							Action<string> onInputText = this.OnInputText;
							if (onInputText != null)
							{
								onInputText(obj);
							}
							this.RedrawInputLine();
							return;
						}
					}
					else
					{
						Func<string, string[]> completion = this.Completion;
						string[] array = (completion != null) ? completion(this.inputString) : null;
						if (array == null || array.Length == 0)
						{
							return;
						}
						if (array.Length > 1)
						{
							this.ClearLine(Interface.Oxide.Config.Console.ShowStatusBar ? (this.StatusTextLeft.Length + 1) : 1);
							Console.ForegroundColor = ConsoleColor.Yellow;
							int num = array.Max((string r) => r.Length);
							for (int i = 0; i < array.Length; i++)
							{
								string text = array[i];
								if (i > 0)
								{
									int firstDiffIndex = ConsoleInput.GetFirstDiffIndex(array[0], text);
									if (firstDiffIndex > 0 && firstDiffIndex < num)
									{
										num = firstDiffIndex;
									}
								}
								Console.WriteLine(text);
							}
							if (num > 0)
							{
								this.inputString = array[0].Substring(0, num);
							}
							this.RedrawInputLine();
							return;
						}
						this.inputString = array[0];
						this.RedrawInputLine();
						return;
					}
				}
				else
				{
					if (this.inputString.Length < 1)
					{
						return;
					}
					this.inputString = this.inputString.Substring(0, this.inputString.Length - 1);
					this.RedrawInputLine();
					return;
				}
			}
			else
			{
				if (key == ConsoleKey.Escape)
				{
					this.inputString = string.Empty;
					this.RedrawInputLine();
					return;
				}
				if (key != ConsoleKey.UpArrow)
				{
					if (key == ConsoleKey.DownArrow)
					{
						if (this.inputHistory.Count == 0)
						{
							return;
						}
						if (this.inputHistoryIndex >= this.inputHistory.Count - 1)
						{
							this.inputHistoryIndex = this.inputHistory.Count - 2;
						}
						string text2;
						if (this.inputHistoryIndex >= 0)
						{
							List<string> list = this.inputHistory;
							int num2 = this.inputHistoryIndex;
							this.inputHistoryIndex = num2 - 1;
							text2 = list[num2];
						}
						else
						{
							text2 = string.Empty;
						}
						this.inputString = text2;
						this.RedrawInputLine();
						return;
					}
				}
				else
				{
					if (this.inputHistory.Count == 0)
					{
						return;
					}
					if (this.inputHistoryIndex < 0)
					{
						this.inputHistoryIndex = 0;
					}
					if (this.inputHistoryIndex >= this.inputHistory.Count - 1)
					{
						this.inputHistoryIndex = this.inputHistory.Count - 1;
						this.inputString = this.inputHistory[this.inputHistoryIndex];
						this.RedrawInputLine();
						return;
					}
					List<string> list2 = this.inputHistory;
					int num2 = this.inputHistoryIndex;
					this.inputHistoryIndex = num2 + 1;
					this.inputString = list2[num2];
					this.RedrawInputLine();
					return;
				}
			}
			if (consoleKeyInfo.KeyChar == '\0')
			{
				return;
			}
			this.inputString += consoleKeyInfo.KeyChar;
			this.RedrawInputLine();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00006EB0 File Offset: 0x000050B0
		private static int GetFirstDiffIndex(string str1, string str2)
		{
			if (str1 == null || str2 == null)
			{
				return -1;
			}
			int num = Math.Min(str1.Length, str2.Length);
			for (int i = 0; i < num; i++)
			{
				if (str1[i] != str2[i])
				{
					return i;
				}
			}
			return num;
		}

		// Token: 0x04000064 RID: 100
		private string inputString = string.Empty;

		// Token: 0x04000065 RID: 101
		private readonly List<string> inputHistory = new List<string>();

		// Token: 0x04000066 RID: 102
		private int inputHistoryIndex;

		// Token: 0x04000067 RID: 103
		private float nextUpdate;

		// Token: 0x04000069 RID: 105
		internal readonly string[] StatusTextLeft = new string[]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};

		// Token: 0x0400006A RID: 106
		internal readonly string[] StatusTextRight = new string[]
		{
			string.Empty,
			string.Empty,
			string.Empty,
			string.Empty
		};

		// Token: 0x0400006B RID: 107
		internal readonly ConsoleColor[] StatusTextLeftColor = new ConsoleColor[]
		{
			ConsoleColor.White,
			ConsoleColor.White,
			ConsoleColor.White,
			ConsoleColor.White
		};

		// Token: 0x0400006C RID: 108
		internal readonly ConsoleColor[] StatusTextRightColor = new ConsoleColor[]
		{
			ConsoleColor.White,
			ConsoleColor.White,
			ConsoleColor.White,
			ConsoleColor.White
		};

		// Token: 0x0400006D RID: 109
		public Func<string, string[]> Completion;
	}
}
