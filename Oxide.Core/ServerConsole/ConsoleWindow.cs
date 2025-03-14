using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Oxide.Core.ServerConsole
{
	// Token: 0x0200001B RID: 27
	public class ConsoleWindow
	{
		// Token: 0x06000105 RID: 261
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AllocConsole();

		// Token: 0x06000106 RID: 262
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool AttachConsole(uint dwProcessId);

		// Token: 0x06000107 RID: 263
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool FreeConsole();

		// Token: 0x06000108 RID: 264
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		// Token: 0x06000109 RID: 265
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleOutputCP(uint wCodePageId);

		// Token: 0x0600010A RID: 266
		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleTitle(string lpConsoleTitle);

		// Token: 0x0600010B RID: 267
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		// Token: 0x0600010C RID: 268
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		// Token: 0x0600010D RID: 269
		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		// Token: 0x0600010E RID: 270 RVA: 0x00006FBC File Offset: 0x000051BC
		public static bool Check(bool force = false)
		{
			PlatformID platform = Environment.OSVersion.Platform;
			if (platform <= PlatformID.Win32NT)
			{
				IntPtr moduleHandle = ConsoleWindow.GetModuleHandle("ntdll.dll");
				return !(moduleHandle == IntPtr.Zero) && ConsoleWindow.GetProcAddress(moduleHandle, "wine_get_version") == IntPtr.Zero && (force || ConsoleWindow.GetConsoleWindow() == IntPtr.Zero);
			}
			return false;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x00007021 File Offset: 0x00005221
		public void SetTitle(string title)
		{
			if (title != null)
			{
				ConsoleWindow.SetConsoleTitle(title);
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x00007030 File Offset: 0x00005230
		public bool Initialize()
		{
			if (!ConsoleWindow.AttachConsole(4294967295U))
			{
				ConsoleWindow.AllocConsole();
			}
			if (ConsoleWindow.GetConsoleWindow() == IntPtr.Zero)
			{
				ConsoleWindow.FreeConsole();
				return false;
			}
			this.oldOutput = Console.Out;
			this.oldEncoding = Console.OutputEncoding;
			UTF8Encoding utf8Encoding = new UTF8Encoding(false);
			ConsoleWindow.SetConsoleOutputCP((uint)utf8Encoding.CodePage);
			Console.OutputEncoding = utf8Encoding;
			Stream stream;
			try
			{
				stream = new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), FileAccess.Write);
			}
			catch (Exception)
			{
				stream = Console.OpenStandardOutput();
			}
			Console.SetOut(new StreamWriter(stream, utf8Encoding)
			{
				AutoFlush = true
			});
			return true;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x000070D8 File Offset: 0x000052D8
		public void Shutdown()
		{
			if (this.oldOutput != null)
			{
				Console.SetOut(this.oldOutput);
			}
			if (this.oldEncoding != null)
			{
				ConsoleWindow.SetConsoleOutputCP((uint)this.oldEncoding.CodePage);
				Console.OutputEncoding = this.oldEncoding;
			}
			ConsoleWindow.FreeConsole();
		}

		// Token: 0x0400006E RID: 110
		private const uint ATTACH_PARENT_PROCESS = 4294967295U;

		// Token: 0x0400006F RID: 111
		private const int STD_OUTPUT_HANDLE = -11;

		// Token: 0x04000070 RID: 112
		private TextWriter oldOutput;

		// Token: 0x04000071 RID: 113
		private Encoding oldEncoding;
	}
}
