using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Oxide.Core
{
	// Token: 0x02000017 RID: 23
	public class Utility
	{
		// Token: 0x060000E4 RID: 228 RVA: 0x00006184 File Offset: 0x00004384
		public static void DatafileToProto<T>(string name, bool deleteAfter = true)
		{
			DataFileSystem dataFileSystem = Interface.Oxide.DataFileSystem;
			if (!dataFileSystem.ExistsDatafile(name))
			{
				return;
			}
			if (ProtoStorage.Exists(new string[]
			{
				name
			}))
			{
				Interface.Oxide.LogWarning("Failed to import JSON file: {0} already exists.", new object[]
				{
					name
				});
				return;
			}
			try
			{
				ProtoStorage.Save<T>(dataFileSystem.ReadObject<T>(name), new string[]
				{
					name
				});
				if (deleteAfter)
				{
					File.Delete(dataFileSystem.GetFile(name).Filename);
				}
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException("Failed to convert datafile to proto storage: " + name, ex);
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x00006228 File Offset: 0x00004428
		public static void PrintCallStack()
		{
			Interface.Oxide.LogDebug("CallStack:{0}{1}", new object[]
			{
				Environment.NewLine,
				new StackTrace(1, true)
			});
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x00006254 File Offset: 0x00004454
		public static string FormatBytes(double bytes)
		{
			string arg;
			if (bytes > 1048576.0)
			{
				arg = "mb";
				bytes /= 1048576.0;
			}
			else if (bytes > 1024.0)
			{
				arg = "kb";
				bytes /= 1024.0;
			}
			else
			{
				arg = "b";
			}
			return string.Format("{0:0}{1}", bytes, arg);
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x000062BC File Offset: 0x000044BC
		public static string GetDirectoryName(string name)
		{
			string result;
			try
			{
				name = name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				result = name.Substring(0, name.LastIndexOf(Path.DirectorySeparatorChar));
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x00006308 File Offset: 0x00004508
		public static string GetFileNameWithoutExtension(string value)
		{
			int num = value.Length - 1;
			for (int i = num; i >= 1; i--)
			{
				if (value[i] == '.')
				{
					num = i - 1;
					break;
				}
			}
			int num2 = 0;
			for (int j = num - 1; j >= 0; j--)
			{
				char c = value[j];
				if (c == '/' || c == '\\')
				{
					num2 = j + 1;
					break;
				}
			}
			return value.Substring(num2, num - num2 + 1);
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x00006372 File Offset: 0x00004572
		public static string CleanPath(string path)
		{
			if (path == null)
			{
				return null;
			}
			return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
		}

		// Token: 0x060000EA RID: 234 RVA: 0x00006392 File Offset: 0x00004592
		public static T ConvertFromJson<T>(string jsonstr)
		{
			return JsonConvert.DeserializeObject<T>(jsonstr);
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000639A File Offset: 0x0000459A
		public static string ConvertToJson(object obj, bool indented = false)
		{
			return JsonConvert.SerializeObject(obj, indented ? Formatting.Indented : Formatting.None);
		}

		// Token: 0x060000EC RID: 236 RVA: 0x000063AC File Offset: 0x000045AC
		public static IPAddress GetLocalIP()
		{
			UnicastIPAddressInformation unicastIPAddressInformation = null;
			foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
			{
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
					if (ipproperties.GatewayAddresses.Count != 0 && !ipproperties.GatewayAddresses[0].Address.Equals(IPAddress.Parse("0.0.0.0")))
					{
						foreach (UnicastIPAddressInformation unicastIPAddressInformation2 in ipproperties.UnicastAddresses)
						{
							if (unicastIPAddressInformation2.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(unicastIPAddressInformation2.Address))
							{
								if (!unicastIPAddressInformation2.IsDnsEligible)
								{
									if (unicastIPAddressInformation == null)
									{
										unicastIPAddressInformation = unicastIPAddressInformation2;
									}
								}
								else
								{
									if (unicastIPAddressInformation2.PrefixOrigin == PrefixOrigin.Dhcp)
									{
										return unicastIPAddressInformation2.Address;
									}
									if (unicastIPAddressInformation == null || !unicastIPAddressInformation.IsDnsEligible)
									{
										unicastIPAddressInformation = unicastIPAddressInformation2;
									}
								}
							}
						}
					}
				}
			}
			if (unicastIPAddressInformation == null)
			{
				return null;
			}
			return unicastIPAddressInformation.Address;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x000064C8 File Offset: 0x000046C8
		public static bool IsLocalIP(string ipAddress)
		{
			string[] array = ipAddress.Split(new string[]
			{
				"."
			}, StringSplitOptions.RemoveEmptyEntries);
			int[] array2 = new int[]
			{
				int.Parse(array[0]),
				int.Parse(array[1]),
				int.Parse(array[2]),
				int.Parse(array[3])
			};
			return array2[0] == 0 || array2[0] == 10 || array2[0] == 127 || (array2[0] == 192 && array2[1] == 168) || (array2[0] == 172 && array2[1] >= 16 && array2[1] <= 31);
		}

		// Token: 0x060000EE RID: 238 RVA: 0x00006564 File Offset: 0x00004764
		public static bool ValidateIPv4(string ipAddress)
		{
			if (string.IsNullOrEmpty(ipAddress.Trim()))
			{
				return false;
			}
			string[] array = ipAddress.Replace("\"", string.Empty).Trim().Split(new char[]
			{
				'.'
			});
			if (array.Length == 4)
			{
				return array.All(delegate(string r)
				{
					byte b;
					return byte.TryParse(r, out b);
				});
			}
			return false;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x000065D4 File Offset: 0x000047D4
		public static int GetNumbers(string input)
		{
			int result;
			int.TryParse(Regex.Replace(input, "[^.0-9]", ""), out result);
			return result;
		}
	}
}
