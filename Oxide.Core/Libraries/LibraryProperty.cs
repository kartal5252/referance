using System;

namespace Oxide.Core.Libraries
{
	// Token: 0x02000038 RID: 56
	[AttributeUsage(AttributeTargets.Property)]
	public class LibraryProperty : Attribute
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x0600020D RID: 525 RVA: 0x0000A7A4 File Offset: 0x000089A4
		// (set) Token: 0x0600020E RID: 526 RVA: 0x0000A7AC File Offset: 0x000089AC
		public string Name { get; private set; }

		// Token: 0x0600020F RID: 527 RVA: 0x0000A7B5 File Offset: 0x000089B5
		public LibraryProperty()
		{
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000A7BD File Offset: 0x000089BD
		public LibraryProperty(string name)
		{
			this.Name = name;
		}
	}
}
