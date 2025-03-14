using System;
using System.Data.Common;
using System.Security.Permissions;
using Oxide.Core.Plugins;

namespace Oxide.Core.Database
{
	// Token: 0x02000054 RID: 84
	[ReflectionPermission(SecurityAction.Deny, Flags = ReflectionPermissionFlag.AllFlags)]
	public sealed class Connection
	{
		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000339 RID: 825 RVA: 0x0000DDA0 File Offset: 0x0000BFA0
		// (set) Token: 0x0600033A RID: 826 RVA: 0x0000DDA8 File Offset: 0x0000BFA8
		public string ConnectionString { get; set; }

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600033B RID: 827 RVA: 0x0000DDB1 File Offset: 0x0000BFB1
		// (set) Token: 0x0600033C RID: 828 RVA: 0x0000DDB9 File Offset: 0x0000BFB9
		public bool ConnectionPersistent { get; set; }

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x0600033D RID: 829 RVA: 0x0000DDC2 File Offset: 0x0000BFC2
		// (set) Token: 0x0600033E RID: 830 RVA: 0x0000DDCA File Offset: 0x0000BFCA
		public DbConnection Con { get; set; }

		// Token: 0x1700008D RID: 141
		// (get) Token: 0x0600033F RID: 831 RVA: 0x0000DDD3 File Offset: 0x0000BFD3
		// (set) Token: 0x06000340 RID: 832 RVA: 0x0000DDDB File Offset: 0x0000BFDB
		public Plugin Plugin { get; set; }

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x06000341 RID: 833 RVA: 0x0000DDE4 File Offset: 0x0000BFE4
		// (set) Token: 0x06000342 RID: 834 RVA: 0x0000DDEC File Offset: 0x0000BFEC
		public long LastInsertRowId { get; set; }

		// Token: 0x06000343 RID: 835 RVA: 0x0000DDF5 File Offset: 0x0000BFF5
		public Connection(string connection, bool persistent)
		{
			this.ConnectionString = connection;
			this.ConnectionPersistent = persistent;
		}
	}
}
