using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Oxide.Core
{
	// Token: 0x02000005 RID: 5
	public sealed class CommandLine
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002438 File Offset: 0x00000638
		public CommandLine(string[] commandline)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			foreach (string text3 in commandline)
			{
				text = text + "\"" + text3.Trim(new char[]
				{
					'/',
					'\\'
				}) + "\"";
			}
			foreach (string text4 in CommandLine.Split(text))
			{
				if (text4.Length > 0)
				{
					string text5 = text4;
					if (text4[0] == '-' || text4[0] == '+')
					{
						if (text2 != string.Empty && !this.variables.ContainsKey(text2))
						{
							this.variables.Add(text2, string.Empty);
						}
						text2 = text5.Substring(1);
					}
					else if (text2 != string.Empty)
					{
						if (!this.variables.ContainsKey(text2))
						{
							if (text2.Contains("dir"))
							{
								text5 = text5.Replace('/', '\\');
							}
							this.variables.Add(text2, text5);
						}
						text2 = string.Empty;
					}
				}
			}
			if (text2 != string.Empty && !this.variables.ContainsKey(text2))
			{
				this.variables.Add(text2, string.Empty);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002594 File Offset: 0x00000794
		public static string[] Split(string input)
		{
			input = input.Replace("\\\"", "&quot;");
			MatchCollection matchCollection = new Regex("\"([^\"]+)\"|'([^']+)'|\\S+").Matches(input);
			string[] array = new string[matchCollection.Count];
			for (int i = 0; i < matchCollection.Count; i++)
			{
				char[] trimChars = new char[]
				{
					' ',
					'"'
				};
				array[i] = matchCollection[i].Groups[0].Value.Trim(trimChars);
				array[i] = array[i].Replace("&quot;", "\"");
			}
			return array;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002628 File Offset: 0x00000828
		public bool HasVariable(string name)
		{
			return this.variables.Any((KeyValuePair<string, string> v) => v.Key == name);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000265C File Offset: 0x0000085C
		public string GetVariable(string name)
		{
			string result;
			try
			{
				result = this.variables.Single((KeyValuePair<string, string> v) => v.Key == name).Value;
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000026B0 File Offset: 0x000008B0
		public void GetArgument(string var, out string varname, out string format)
		{
			string variable = this.GetVariable(var);
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			int num = 0;
			foreach (char c in variable)
			{
				if (c != '{')
				{
					if (c != '}')
					{
						if (num == 0)
						{
							stringBuilder2.Append(c);
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					else
					{
						num--;
						if (num == 0)
						{
							stringBuilder2.Append("{0}");
						}
					}
				}
				else
				{
					num++;
				}
			}
			varname = stringBuilder.ToString();
			format = stringBuilder2.ToString();
		}

		// Token: 0x0400000D RID: 13
		private readonly Dictionary<string, string> variables = new Dictionary<string, string>();
	}
}
