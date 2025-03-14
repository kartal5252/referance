using System;

namespace Oxide.Core.Plugins
{
	// Token: 0x0200001F RID: 31
	[AttributeUsage(AttributeTargets.Method)]
	public class HookMethodAttribute : Attribute
	{
		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000140 RID: 320 RVA: 0x00007ADD File Offset: 0x00005CDD
		public string Name { get; }

		// Token: 0x06000141 RID: 321 RVA: 0x00007AE5 File Offset: 0x00005CE5
		public HookMethodAttribute(string name)
		{
			this.Name = name;
		}
	}
}
