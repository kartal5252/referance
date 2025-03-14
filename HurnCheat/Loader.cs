using System;
using UnityEngine;

namespace HurnCheat
{
	// Token: 0x02000003 RID: 3
	internal class Loader
	{
		// Token: 0x0600000E RID: 14 RVA: 0x00004083 File Offset: 0x00002283
		public static void Load()
		{
			Loader.load_object = new GameObject();
			Loader.load_object.AddComponent<Main>();
			Object.DontDestroyOnLoad(Loader.load_object);
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000040A4 File Offset: 0x000022A4
		public static void UnLoad()
		{
			Object.Destroy(Loader.load_object);
		}

		// Token: 0x04000043 RID: 67
		public static GameObject load_object;
	}
}
