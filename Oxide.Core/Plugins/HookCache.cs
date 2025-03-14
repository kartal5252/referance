using System;
using System.Collections.Generic;

namespace Oxide.Core.Plugins
{
	// Token: 0x02000021 RID: 33
	public class HookCache
	{
		// Token: 0x06000149 RID: 329 RVA: 0x00008048 File Offset: 0x00006248
		public List<HookMethod> GetHookMethod(string hookName, object[] args, out HookCache cache)
		{
			HookCache hookCache;
			if (!this._cache.TryGetValue(hookName, out hookCache))
			{
				hookCache = new HookCache();
				this._cache.Add(hookName, hookCache);
			}
			return hookCache.GetHookMethod(args, 0, out cache);
		}

		// Token: 0x0600014A RID: 330 RVA: 0x00008084 File Offset: 0x00006284
		public List<HookMethod> GetHookMethod(object[] args, int index, out HookCache cache)
		{
			if (args == null || index >= args.Length)
			{
				cache = this;
				return this._methods;
			}
			HookCache hookCache;
			if (args[index] == null)
			{
				if (!this._cache.TryGetValue(this.NullKey, out hookCache))
				{
					hookCache = new HookCache();
					this._cache.Add(this.NullKey, hookCache);
				}
			}
			else if (!this._cache.TryGetValue(args[index].GetType().FullName, out hookCache))
			{
				hookCache = new HookCache();
				this._cache.Add(args[index].GetType().FullName, hookCache);
			}
			return hookCache.GetHookMethod(args, index + 1, out cache);
		}

		// Token: 0x0600014B RID: 331 RVA: 0x0000811E File Offset: 0x0000631E
		public void SetupMethods(List<HookMethod> methods)
		{
			this._methods = methods;
		}

		// Token: 0x0400008A RID: 138
		private string NullKey = "null";

		// Token: 0x0400008B RID: 139
		public Dictionary<string, HookCache> _cache = new Dictionary<string, HookCache>();

		// Token: 0x0400008C RID: 140
		public List<HookMethod> _methods;
	}
}
