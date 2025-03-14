using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Oxide.Core.Libraries.Covalence;

namespace Oxide.Core
{
	// Token: 0x0200000E RID: 14
	public static class ExtensionMethods
	{
		// Token: 0x0600004C RID: 76 RVA: 0x00003970 File Offset: 0x00001B70
		public static string Basename(this string text, string extension = null)
		{
			if (extension != null)
			{
				if (!extension.Equals("*.*"))
				{
					if (extension[0] == '*')
					{
						extension = extension.Substring(1);
					}
					return Regex.Match(text, "([^\\\\/]+)\\" + extension + "+$").Groups[1].Value;
				}
				Match match = Regex.Match(text, "([^\\\\/]+)\\.[^\\.]+$");
				if (match.Success)
				{
					return match.Groups[1].Value;
				}
			}
			return Regex.Match(text, "[^\\\\/]+$").Groups[0].Value;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003A0C File Offset: 0x00001C0C
		public static bool Contains<T>(this T[] array, T value)
		{
			foreach (T t in array)
			{
				if (t.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003A49 File Offset: 0x00001C49
		public static string Dirname(this string text)
		{
			return Regex.Match(text, "(.+)[\\/][^\\/]+$").Groups[1].Value;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003A66 File Offset: 0x00001C66
		public static string Humanize(this string name)
		{
			return Regex.Replace(name, "(\\B[A-Z])", " $1");
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003A78 File Offset: 0x00001C78
		public static bool IsSteamId(this string id)
		{
			ulong num;
			return ulong.TryParse(id, out num) && num > 76561197960265728UL;
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00003A9D File Offset: 0x00001C9D
		public static bool IsSteamId(this ulong id)
		{
			return id > 76561197960265728UL;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00003AAB File Offset: 0x00001CAB
		public static string Plaintext(this string text)
		{
			return Formatter.ToPlaintext(text);
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00003AB3 File Offset: 0x00001CB3
		public static string QuoteSafe(this string text)
		{
			return "\"" + text.Replace("\"", "\\\"").TrimEnd(new char[]
			{
				'\\'
			}) + "\"";
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00003AE4 File Offset: 0x00001CE4
		public static string Quote(this string text)
		{
			return text.QuoteSafe();
		}

		// Token: 0x06000055 RID: 85 RVA: 0x00003AEC File Offset: 0x00001CEC
		public static T Sample<T>(this T[] array)
		{
			return array[Random.Range(0, array.Length)];
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00003AFD File Offset: 0x00001CFD
		public static string Sanitize(this string text)
		{
			return text.Replace("{", "{{").Replace("}", "}}");
		}

		// Token: 0x06000057 RID: 87 RVA: 0x00003B1E File Offset: 0x00001D1E
		public static string SentenceCase(this string text)
		{
			return new Regex("(^[a-z])|\\.\\s+(.)", RegexOptions.ExplicitCapture).Replace(text.ToLower(), (Match s) => s.Value.ToUpper());
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00003B55 File Offset: 0x00001D55
		public static string TitleCase(this string text)
		{
			return CultureInfo.InstalledUICulture.TextInfo.ToTitleCase(text.Contains('_') ? text.Replace('_', ' ') : text);
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00003B7D File Offset: 0x00001D7D
		public static string Titleize(this string text)
		{
			return text.TitleCase();
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00003B88 File Offset: 0x00001D88
		public static string ToSentence<T>(this IEnumerable<T> items)
		{
			IEnumerator<T> enumerator = items.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				return string.Empty;
			}
			T t = enumerator.Current;
			if (enumerator.MoveNext())
			{
				StringBuilder stringBuilder = new StringBuilder((t != null) ? t.ToString() : null);
				bool flag = true;
				while (flag)
				{
					T t2 = enumerator.Current;
					flag = enumerator.MoveNext();
					stringBuilder.Append(flag ? ", " : " and ");
					stringBuilder.Append(t2);
				}
				return stringBuilder.ToString();
			}
			if (t == null)
			{
				return null;
			}
			return t.ToString();
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003C2F File Offset: 0x00001E2F
		public static string Truncate(this string text, int max)
		{
			if (text.Length > max)
			{
				return text.Substring(0, max) + " ...";
			}
			return text;
		}
	}
}
