using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x0200000D RID: 13
	public class ExceptionHandler
	{
		// Token: 0x06000048 RID: 72 RVA: 0x00003922 File Offset: 0x00001B22
		public static void RegisterType(Type ex, Func<Exception, string> handler)
		{
			ExceptionHandler.Handlers[ex] = handler;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00003930 File Offset: 0x00001B30
		public static string FormatException(Exception ex)
		{
			Func<Exception, string> func;
			if (!ExceptionHandler.Handlers.TryGetValue(ex.GetType(), out func))
			{
				return null;
			}
			return func(ex);
		}

		// Token: 0x0400002E RID: 46
		private static readonly Dictionary<Type, Func<Exception, string>> Handlers = new Dictionary<Type, Func<Exception, string>>();
	}
}
