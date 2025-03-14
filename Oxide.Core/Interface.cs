using System;

namespace Oxide.Core
{
	// Token: 0x0200000F RID: 15
	public static class Interface
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00003C4E File Offset: 0x00001E4E
		// (set) Token: 0x0600005D RID: 93 RVA: 0x00003C55 File Offset: 0x00001E55
		public static OxideMod Oxide { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00003C5D File Offset: 0x00001E5D
		// (set) Token: 0x0600005F RID: 95 RVA: 0x00003C64 File Offset: 0x00001E64
		public static NativeDebugCallback DebugCallback { get; set; }

		// Token: 0x06000060 RID: 96 RVA: 0x00003C6C File Offset: 0x00001E6C
		public static void Initialize()
		{
			if (Interface.Oxide != null)
			{
				return;
			}
			Interface.Oxide = new OxideMod(Interface.DebugCallback);
			Interface.Oxide.Load();
		}

		// Token: 0x06000061 RID: 97 RVA: 0x00003C8F File Offset: 0x00001E8F
		public static object CallDeprecatedHook(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			return Interface.Oxide.CallDeprecatedHook(oldHook, newHook, expireDate, args);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003C9F File Offset: 0x00001E9F
		public static object CallDeprecated(string oldHook, string newHook, DateTime expireDate, params object[] args)
		{
			return Interface.CallDeprecatedHook(oldHook, newHook, expireDate, args);
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003CAA File Offset: 0x00001EAA
		public static object CallHook(string hook, object[] args)
		{
			OxideMod oxide = Interface.Oxide;
			if (oxide == null)
			{
				return null;
			}
			return oxide.CallHook(hook, args);
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00003CBE File Offset: 0x00001EBE
		public static object CallHook(string hook)
		{
			return Interface.CallHook(hook, null);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00003CC8 File Offset: 0x00001EC8
		public static object CallHook(string hook, object obj1)
		{
			object[] array = ArrayPool.Get(1);
			array[0] = obj1;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00003CF0 File Offset: 0x00001EF0
		public static object CallHook(string hook, object obj1, object obj2)
		{
			object[] array = ArrayPool.Get(2);
			array[0] = obj1;
			array[1] = obj2;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00003D1C File Offset: 0x00001F1C
		public static object CallHook(string hook, object obj1, object obj2, object obj3)
		{
			object[] array = ArrayPool.Get(3);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00003D4C File Offset: 0x00001F4C
		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4)
		{
			object[] array = ArrayPool.Get(4);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			array[3] = obj4;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00003D80 File Offset: 0x00001F80
		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5)
		{
			object[] array = ArrayPool.Get(5);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			array[3] = obj4;
			array[4] = obj5;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00003DB8 File Offset: 0x00001FB8
		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6)
		{
			object[] array = ArrayPool.Get(6);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			array[3] = obj4;
			array[4] = obj5;
			array[5] = obj6;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00003DF4 File Offset: 0x00001FF4
		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7)
		{
			object[] array = ArrayPool.Get(7);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			array[3] = obj4;
			array[4] = obj5;
			array[5] = obj6;
			array[6] = obj7;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00003E38 File Offset: 0x00002038
		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8)
		{
			object[] array = ArrayPool.Get(8);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			array[3] = obj4;
			array[4] = obj5;
			array[5] = obj6;
			array[6] = obj7;
			array[7] = obj8;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00003E80 File Offset: 0x00002080
		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, object obj9)
		{
			object[] array = ArrayPool.Get(9);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			array[3] = obj4;
			array[4] = obj5;
			array[5] = obj6;
			array[6] = obj7;
			array[7] = obj8;
			array[8] = obj9;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00003ECC File Offset: 0x000020CC
		public static object CallHook(string hook, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, object obj9, object obj10)
		{
			object[] array = ArrayPool.Get(10);
			array[0] = obj1;
			array[1] = obj2;
			array[2] = obj3;
			array[3] = obj4;
			array[4] = obj5;
			array[5] = obj6;
			array[6] = obj7;
			array[7] = obj8;
			array[8] = obj9;
			array[9] = obj10;
			object result = Interface.CallHook(hook, array);
			ArrayPool.Free(array);
			return result;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003F1E File Offset: 0x0000211E
		public static object Call(string hook, params object[] args)
		{
			return Interface.CallHook(hook, args);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00003F27 File Offset: 0x00002127
		public static T Call<T>(string hook, params object[] args)
		{
			return (T)((object)Convert.ChangeType(Interface.CallHook(hook, args), typeof(T)));
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00003F44 File Offset: 0x00002144
		public static OxideMod GetMod()
		{
			return Interface.Oxide;
		}
	}
}
