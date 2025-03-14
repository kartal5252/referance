using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x02000003 RID: 3
	public static class ArrayPool
	{
		// Token: 0x06000005 RID: 5 RVA: 0x00002210 File Offset: 0x00000410
		static ArrayPool()
		{
			for (int i = 0; i < 50; i++)
			{
				ArrayPool._pooledArrays.Add(new Queue<object[]>());
				ArrayPool.SetupArrays(i + 1);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000224C File Offset: 0x0000044C
		public static object[] Get(int length)
		{
			if (length == 0 || length > 50)
			{
				return new object[length];
			}
			Queue<object[]> queue = ArrayPool._pooledArrays[length - 1];
			Queue<object[]> obj = queue;
			object[] result;
			lock (obj)
			{
				if (queue.Count == 0)
				{
					ArrayPool.SetupArrays(length);
				}
				result = queue.Dequeue();
			}
			return result;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000022B0 File Offset: 0x000004B0
		public static void Free(object[] array)
		{
			if (array == null || array.Length == 0 || array.Length > 50)
			{
				return;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = null;
			}
			Queue<object[]> queue = ArrayPool._pooledArrays[array.Length - 1];
			Queue<object[]> obj = queue;
			lock (obj)
			{
				if (queue.Count > 256)
				{
					for (int j = 0; j < 64; j++)
					{
						queue.Dequeue();
					}
				}
				else
				{
					queue.Enqueue(array);
				}
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x0000233C File Offset: 0x0000053C
		private static void SetupArrays(int length)
		{
			Queue<object[]> queue = ArrayPool._pooledArrays[length - 1];
			for (int i = 0; i < 64; i++)
			{
				queue.Enqueue(new object[length]);
			}
		}

		// Token: 0x04000008 RID: 8
		private const int MaxArrayLength = 50;

		// Token: 0x04000009 RID: 9
		private const int InitialPoolAmount = 64;

		// Token: 0x0400000A RID: 10
		private const int MaxPoolAmount = 256;

		// Token: 0x0400000B RID: 11
		private static List<Queue<object[]>> _pooledArrays = new List<Queue<object[]>>();
	}
}
