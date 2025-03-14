using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Oxide.Core.Extensions;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;

namespace Oxide.Core
{
	// Token: 0x02000015 RID: 21
	public static class RemoteLogger
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x00005934 File Offset: 0x00003B34
		private static Dictionary<string, string> BuildHeaders()
		{
			string text = string.Join(", ", (from x in RemoteLogger.sentryAuth
			select string.Join("=", x)).ToArray<string>());
			text = text + ", sentry_timestamp=" + (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			return new Dictionary<string, string>
			{
				{
					"X-Sentry-Auth",
					"Sentry " + text
				}
			};
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000059C8 File Offset: 0x00003BC8
		public static void SetTag(string name, string value)
		{
			RemoteLogger.Tags[name] = value;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x000059D8 File Offset: 0x00003BD8
		public static string GetTag(string name)
		{
			string result;
			if (!RemoteLogger.Tags.TryGetValue(name, out result))
			{
				return "unknown";
			}
			return result;
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x000059FB File Offset: 0x00003BFB
		public static void Debug(string message)
		{
			RemoteLogger.EnqueueReport("debug", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00005A13 File Offset: 0x00003C13
		public static void Error(string message)
		{
			RemoteLogger.EnqueueReport("error", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00005A2B File Offset: 0x00003C2B
		public static void Info(string message)
		{
			RemoteLogger.EnqueueReport("info", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005A43 File Offset: 0x00003C43
		public static void Warning(string message)
		{
			RemoteLogger.EnqueueReport("warning", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, null);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005A5C File Offset: 0x00003C5C
		public static void Exception(string message, Exception exception)
		{
			if (!exception.StackTrace.Contains("Oxide.Core") && !exception.StackTrace.Contains("Oxide.Plugins.Compiler"))
			{
				return;
			}
			foreach (string value in RemoteLogger.ExceptionFilter)
			{
				if (exception.StackTrace.Contains(value) || message.Contains(value))
				{
					return;
				}
			}
			RemoteLogger.EnqueueReport("fatal", Assembly.GetCallingAssembly(), RemoteLogger.GetCurrentMethod(), message, exception.ToString());
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00005ADC File Offset: 0x00003CDC
		public static void Exception(string message, string rawStackTrace)
		{
			string[] array = rawStackTrace.Split(new char[]
			{
				'\r',
				'\n'
			});
			string culprit = array[0].Split(new char[]
			{
				'('
			})[0].Trim();
			RemoteLogger.EnqueueReport("fatal", array, culprit, message, rawStackTrace);
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00005B29 File Offset: 0x00003D29
		private static void EnqueueReport(string level, Assembly assembly, string culprit, string message, string exception = null)
		{
			RemoteLogger.Report report = new RemoteLogger.Report(level, culprit, message, exception);
			report.DetectModules(assembly);
			RemoteLogger.EnqueueReport(report);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00005B41 File Offset: 0x00003D41
		private static void EnqueueReport(string level, string[] stackTrace, string culprit, string message, string exception = null)
		{
			RemoteLogger.Report report = new RemoteLogger.Report(level, culprit, message, exception);
			report.DetectModules(stackTrace);
			RemoteLogger.EnqueueReport(report);
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x00005B5C File Offset: 0x00003D5C
		private static void EnqueueReport(RemoteLogger.Report report)
		{
			Dictionary<string, string>.ValueCollection values = report.extra.Values;
			if (!values.Contains("Oxide.Core") && !values.Contains("Oxide.Plugins.Compiler"))
			{
				return;
			}
			foreach (string value in RemoteLogger.ExceptionFilter)
			{
				if (values.Contains(value) || values.Contains(value))
				{
					return;
				}
			}
			RemoteLogger.QueuedReports.Add(new RemoteLogger.QueuedReport(report));
			if (!RemoteLogger.submittingReports)
			{
				RemoteLogger.SubmitNextReport();
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00005BD8 File Offset: 0x00003DD8
		private static void SubmitNextReport()
		{
			if (RemoteLogger.QueuedReports.Count < 1)
			{
				return;
			}
			RemoteLogger.QueuedReport queuedReport = RemoteLogger.QueuedReports[0];
			RemoteLogger.submittingReports = true;
			RemoteLogger.Webrequests.Enqueue(RemoteLogger.Url, queuedReport.Body, delegate(int code, string response)
			{
				if (code == 200)
				{
					RemoteLogger.QueuedReports.RemoveAt(0);
					RemoteLogger.submittingReports = false;
					RemoteLogger.SubmitNextReport();
					return;
				}
				RemoteLogger.Timers.Once(5f, new Action(RemoteLogger.SubmitNextReport), null);
			}, null, RequestMethod.POST, queuedReport.Headers, 0f);
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x00005C48 File Offset: 0x00003E48
		[MethodImpl(MethodImplOptions.NoInlining)]
		private static string GetCurrentMethod()
		{
			MethodBase method = new StackTrace().GetFrame(2).GetMethod();
			Type declaringType = method.DeclaringType;
			return ((declaringType != null) ? declaringType.FullName : null) + "." + method.Name;
		}

		// Token: 0x04000052 RID: 82
		private const int projectId = 141692;

		// Token: 0x04000053 RID: 83
		private const string host = "sentry.io";

		// Token: 0x04000054 RID: 84
		private const string publicKey = "2d0162c790be4036a94d2d8326d7f900";

		// Token: 0x04000055 RID: 85
		private const string secretKey = "8a6249aad4b84e368f900b32396e8b04";

		// Token: 0x04000056 RID: 86
		private static readonly string Url = "https://sentry.io/api/" + 141692 + "/store/";

		// Token: 0x04000057 RID: 87
		private static readonly string[][] sentryAuth = new string[][]
		{
			new string[]
			{
				"sentry_version",
				"7"
			},
			new string[]
			{
				"sentry_client",
				"MiniRaven/1.0"
			},
			new string[]
			{
				"sentry_key",
				"2d0162c790be4036a94d2d8326d7f900"
			},
			new string[]
			{
				"sentry_secret",
				"8a6249aad4b84e368f900b32396e8b04"
			}
		};

		// Token: 0x04000058 RID: 88
		public static string Filename = Utility.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName);

		// Token: 0x04000059 RID: 89
		private static readonly Dictionary<string, string> Tags = new Dictionary<string, string>
		{
			{
				"arch",
				(IntPtr.Size == 8) ? "x64" : "x86"
			},
			{
				"platform",
				Environment.OSVersion.Platform.ToString().ToLower()
			},
			{
				"os version",
				Environment.OSVersion.Version.ToString().ToLower()
			},
			{
				"game",
				RemoteLogger.Filename.ToLower().Replace("dedicated", "").Replace("server", "").Replace("-", "").Replace("_", "")
			}
		};

		// Token: 0x0400005A RID: 90
		private static readonly Timer Timers = Interface.Oxide.GetLibrary<Timer>(null);

		// Token: 0x0400005B RID: 91
		private static readonly WebRequests Webrequests = Interface.Oxide.GetLibrary<WebRequests>(null);

		// Token: 0x0400005C RID: 92
		private static readonly List<RemoteLogger.QueuedReport> QueuedReports = new List<RemoteLogger.QueuedReport>();

		// Token: 0x0400005D RID: 93
		private static bool submittingReports;

		// Token: 0x0400005E RID: 94
		public static string[] ExceptionFilter = new string[]
		{
			"BadImageFormatException",
			"DllNotFoundException",
			"FileNotFoundException",
			"IOException",
			"KeyNotFoundException",
			"Oxide.Core.Configuration",
			"Oxide.Ext.",
			"Oxide.Plugins.<",
			"ReflectionTypeLoadException",
			"Sharing violation",
			"UnauthorizedAccessException",
			"WebException"
		};

		// Token: 0x02000070 RID: 112
		private class QueuedReport
		{
			// Token: 0x060003C8 RID: 968 RVA: 0x0000F6E0 File Offset: 0x0000D8E0
			public QueuedReport(RemoteLogger.Report report)
			{
				this.Headers = RemoteLogger.BuildHeaders();
				this.Body = JsonConvert.SerializeObject(report);
			}

			// Token: 0x0400017A RID: 378
			public readonly Dictionary<string, string> Headers;

			// Token: 0x0400017B RID: 379
			public readonly string Body;
		}

		// Token: 0x02000071 RID: 113
		public class Report
		{
			// Token: 0x060003C9 RID: 969 RVA: 0x0000F700 File Offset: 0x0000D900
			public Report(string level, string culprit, string message, string exception = null)
			{
				this.headers = RemoteLogger.BuildHeaders();
				this.level = level;
				this.message = ((message.Length > 1000) ? message.Substring(0, 1000) : message);
				this.culprit = culprit;
				this.modules = new Dictionary<string, string>();
				foreach (Extension extension in Interface.Oxide.GetAllExtensions())
				{
					this.modules[extension.GetType().Assembly.GetName().Name] = extension.Version.ToString();
				}
				if (exception != null)
				{
					this.extra = new Dictionary<string, string>();
					string[] array = exception.Split(new char[]
					{
						'\n'
					}).Take(31).ToArray<string>();
					for (int i = 0; i < array.Length; i++)
					{
						string text = array[i].Trim(new char[]
						{
							' ',
							'\r',
							'\n'
						}).Replace('\t', ' ');
						if (text.Length > 0)
						{
							this.extra["line_" + i.ToString("00")] = text;
						}
					}
				}
			}

			// Token: 0x060003CA RID: 970 RVA: 0x0000F890 File Offset: 0x0000DA90
			public void DetectModules(Assembly assembly)
			{
				if (assembly.GetTypes().FirstOrDefault((Type t) => t.BaseType == typeof(Extension)) == null)
				{
					Type type = assembly.GetTypes().FirstOrDefault((Type t) => RemoteLogger.Report.IsTypeDerivedFrom(t, typeof(Plugin)));
					if (type != null)
					{
						Plugin plugin = Interface.Oxide.RootPluginManager.GetPlugin(type.Name);
						if (plugin != null)
						{
							this.modules["Plugins." + plugin.Name] = plugin.Version.ToString();
						}
					}
				}
			}

			// Token: 0x060003CB RID: 971 RVA: 0x0000F940 File Offset: 0x0000DB40
			public void DetectModules(string[] stackTrace)
			{
				int i = 0;
				while (i < stackTrace.Length)
				{
					string text = stackTrace[i];
					if (text.StartsWith("Oxide.Plugins.PluginCompiler") && text.Contains("+"))
					{
						string name = text.Split(new char[]
						{
							'+'
						})[0];
						Plugin plugin = Interface.Oxide.RootPluginManager.GetPlugin(name);
						if (plugin != null)
						{
							this.modules["Plugins." + plugin.Name] = plugin.Version.ToString();
							return;
						}
						break;
					}
					else
					{
						i++;
					}
				}
			}

			// Token: 0x060003CC RID: 972 RVA: 0x0000F9DB File Offset: 0x0000DBDB
			private static bool IsTypeDerivedFrom(Type type, Type baseType)
			{
				while (type != null && type != baseType)
				{
					if ((type = type.BaseType) == baseType)
					{
						return true;
					}
				}
				return false;
			}

			// Token: 0x0400017C RID: 380
			public string message;

			// Token: 0x0400017D RID: 381
			public string level;

			// Token: 0x0400017E RID: 382
			public string culprit;

			// Token: 0x0400017F RID: 383
			public string platform = "csharp";

			// Token: 0x04000180 RID: 384
			public string release = OxideMod.Version.ToString();

			// Token: 0x04000181 RID: 385
			public Dictionary<string, string> tags = RemoteLogger.Tags;

			// Token: 0x04000182 RID: 386
			public Dictionary<string, string> modules;

			// Token: 0x04000183 RID: 387
			public Dictionary<string, string> extra;

			// Token: 0x04000184 RID: 388
			private Dictionary<string, string> headers;
		}
	}
}
