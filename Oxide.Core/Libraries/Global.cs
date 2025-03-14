using System;

namespace Oxide.Core.Libraries
{
	// Token: 0x02000035 RID: 53
	public class Global : Library
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x060001F6 RID: 502 RVA: 0x0000A128 File Offset: 0x00008328
		public override bool IsGlobal
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000A12B File Offset: 0x0000832B
		[LibraryFunction("V")]
		public VersionNumber MakeVersion(ushort major, ushort minor, ushort patch)
		{
			return new VersionNumber((int)major, (int)minor, (int)patch);
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000A135 File Offset: 0x00008335
		[LibraryFunction("new")]
		public object New(Type type, object[] args)
		{
			if (args != null)
			{
				return Activator.CreateInstance(type, args);
			}
			return Activator.CreateInstance(type);
		}
	}
}
