using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Oxide.Core
{
	// Token: 0x02000013 RID: 19
	public class ProtoStorage
	{
		// Token: 0x060000BA RID: 186 RVA: 0x00005765 File Offset: 0x00003965
		public static IEnumerable<string> GetFiles(string subDirectory)
		{
			string fileDataPath = ProtoStorage.GetFileDataPath(subDirectory.Replace("..", ""));
			if (!Directory.Exists(fileDataPath))
			{
				yield break;
			}
			foreach (string value in Directory.GetFiles(fileDataPath, "*.data"))
			{
				yield return Utility.GetFileNameWithoutExtension(value);
			}
			string[] array = null;
			yield break;
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005778 File Offset: 0x00003978
		public static T Load<T>(params string[] subPaths)
		{
			string fileName = ProtoStorage.GetFileName(subPaths);
			string fileDataPath = ProtoStorage.GetFileDataPath(fileName);
			try
			{
				if (File.Exists(fileDataPath))
				{
					T result;
					using (FileStream fileStream = File.OpenRead(fileDataPath))
					{
						result = Serializer.Deserialize<T>(fileStream);
					}
					return result;
				}
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException("Failed to load protobuf data from " + fileName, ex);
			}
			return default(T);
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00005800 File Offset: 0x00003A00
		public static void Save<T>(T data, params string[] subPaths)
		{
			string fileName = ProtoStorage.GetFileName(subPaths);
			string fileDataPath = ProtoStorage.GetFileDataPath(fileName);
			string directoryName = Path.GetDirectoryName(fileDataPath);
			try
			{
				if (directoryName != null && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				using (FileStream fileStream = File.Open(fileDataPath, FileMode.Create))
				{
					Serializer.Serialize<T>(fileStream, data);
				}
			}
			catch (Exception ex)
			{
				Interface.Oxide.LogException("Failed to save protobuf data to " + fileName, ex);
			}
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00005888 File Offset: 0x00003A88
		public static bool Exists(params string[] subPaths)
		{
			return File.Exists(ProtoStorage.GetFileDataPath(ProtoStorage.GetFileName(subPaths)));
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000589C File Offset: 0x00003A9C
		public static string GetFileName(params string[] subPaths)
		{
			return string.Join(Path.DirectorySeparatorChar.ToString(), subPaths).Replace("..", "") + ".data";
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000058D5 File Offset: 0x00003AD5
		public static string GetFileDataPath(string name)
		{
			return Path.Combine(Interface.Oxide.DataDirectory, name);
		}
	}
}
