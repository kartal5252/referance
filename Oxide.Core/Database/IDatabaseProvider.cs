using System;
using System.Collections.Generic;
using Oxide.Core.Plugins;

namespace Oxide.Core.Database
{
	// Token: 0x02000055 RID: 85
	public interface IDatabaseProvider
	{
		// Token: 0x06000344 RID: 836
		Connection OpenDb(string file, Plugin plugin, bool persistent = false);

		// Token: 0x06000345 RID: 837
		void CloseDb(Connection db);

		// Token: 0x06000346 RID: 838
		Sql NewSql();

		// Token: 0x06000347 RID: 839
		void Query(Sql sql, Connection db, Action<List<Dictionary<string, object>>> callback);

		// Token: 0x06000348 RID: 840
		void ExecuteNonQuery(Sql sql, Connection db, Action<int> callback = null);

		// Token: 0x06000349 RID: 841
		void Insert(Sql sql, Connection db, Action<int> callback = null);

		// Token: 0x0600034A RID: 842
		void Update(Sql sql, Connection db, Action<int> callback = null);

		// Token: 0x0600034B RID: 843
		void Delete(Sql sql, Connection db, Action<int> callback = null);
	}
}
