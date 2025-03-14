using System;

namespace Oxide.Core.Libraries
{
	// Token: 0x02000037 RID: 55
	[AttributeUsage(AttributeTargets.Method)]
	public class LibraryFunction : Attribute
	{
		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0000A785 File Offset: 0x00008985
		public string Name { get; }

		// Token: 0x0600020B RID: 523 RVA: 0x0000A78D File Offset: 0x0000898D
		public LibraryFunction()
		{
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000A795 File Offset: 0x00008995
		public LibraryFunction(string name)
		{
			this.Name = name;
		}
	}
}
