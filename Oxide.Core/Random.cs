using System;

namespace Oxide.Core
{
	// Token: 0x02000014 RID: 20
	public static class Random
	{
		// Token: 0x060000C2 RID: 194 RVA: 0x000058FB File Offset: 0x00003AFB
		public static int Range(int min, int max)
		{
			return Random.random.Next(min, max);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00005909 File Offset: 0x00003B09
		public static int Range(int max)
		{
			return Random.random.Next(max);
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005916 File Offset: 0x00003B16
		public static double Range(double min, double max)
		{
			return min + Random.random.NextDouble() * (max - min);
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005928 File Offset: 0x00003B28
		public static float Range(float min, float max)
		{
			return (float)Random.Range((double)min, (double)max);
		}

		// Token: 0x04000051 RID: 81
		private static readonly Random random = new Random();
	}
}
