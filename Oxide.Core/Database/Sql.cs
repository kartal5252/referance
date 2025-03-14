using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Oxide.Core.Database
{
	// Token: 0x02000056 RID: 86
	public class Sql
	{
		// Token: 0x0600034C RID: 844 RVA: 0x0000DE0B File Offset: 0x0000C00B
		public Sql()
		{
		}

		// Token: 0x0600034D RID: 845 RVA: 0x0000DE13 File Offset: 0x0000C013
		public Sql(string sql, params object[] args)
		{
			this._sql = sql;
			this._args = args;
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600034E RID: 846 RVA: 0x0000DE29 File Offset: 0x0000C029
		public static Sql Builder
		{
			get
			{
				return new Sql();
			}
		}

		// Token: 0x17000090 RID: 144
		// (get) Token: 0x0600034F RID: 847 RVA: 0x0000DE30 File Offset: 0x0000C030
		public string SQL
		{
			get
			{
				this.Build();
				return this._sqlFinal;
			}
		}

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000350 RID: 848 RVA: 0x0000DE3E File Offset: 0x0000C03E
		public object[] Arguments
		{
			get
			{
				this.Build();
				return this._argsFinal;
			}
		}

		// Token: 0x06000351 RID: 849 RVA: 0x0000DE4C File Offset: 0x0000C04C
		private void Build()
		{
			if (this._sqlFinal != null)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			List<object> list = new List<object>();
			this.Build(stringBuilder, list, null);
			string text = stringBuilder.ToString();
			if (Sql.Filter.IsMatch(text))
			{
				throw new Exception("Commands LOAD DATA, LOAD_FILE, OUTFILE, DUMPFILE not allowed.");
			}
			this._sqlFinal = text;
			this._argsFinal = list.ToArray();
		}

		// Token: 0x06000352 RID: 850 RVA: 0x0000DEA9 File Offset: 0x0000C0A9
		public Sql Append(Sql sql)
		{
			if (this._rhs != null)
			{
				this._rhs.Append(sql);
			}
			else
			{
				this._rhs = sql;
			}
			return this;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x0000DECA File Offset: 0x0000C0CA
		public Sql Append(string sql, params object[] args)
		{
			return this.Append(new Sql(sql, args));
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000DED9 File Offset: 0x0000C0D9
		private static bool Is(Sql sql, string sqltype)
		{
			return sql != null && sql._sql != null && sql._sql.StartsWith(sqltype, StringComparison.InvariantCultureIgnoreCase);
		}

		// Token: 0x06000355 RID: 853 RVA: 0x0000DEF8 File Offset: 0x0000C0F8
		private void Build(StringBuilder sb, List<object> args, Sql lhs)
		{
			if (!string.IsNullOrEmpty(this._sql))
			{
				if (sb.Length > 0)
				{
					sb.Append("\n");
				}
				string text = Sql.ProcessParams(this._sql, this._args, args);
				if (Sql.Is(lhs, "WHERE ") && Sql.Is(this, "WHERE "))
				{
					text = "AND " + text.Substring(6);
				}
				if (Sql.Is(lhs, "ORDER BY ") && Sql.Is(this, "ORDER BY "))
				{
					text = ", " + text.Substring(9);
				}
				sb.Append(text);
			}
			Sql rhs = this._rhs;
			if (rhs == null)
			{
				return;
			}
			rhs.Build(sb, args, this);
		}

		// Token: 0x06000356 RID: 854 RVA: 0x0000DFB1 File Offset: 0x0000C1B1
		public Sql Where(string sql, params object[] args)
		{
			return this.Append(new Sql("WHERE (" + sql + ")", args));
		}

		// Token: 0x06000357 RID: 855 RVA: 0x0000DFD0 File Offset: 0x0000C1D0
		public Sql OrderBy(params object[] columns)
		{
			return this.Append(new Sql("ORDER BY " + string.Join(", ", (from x in columns
			select x.ToString()).ToArray<string>()), new object[0]));
		}

		// Token: 0x06000358 RID: 856 RVA: 0x0000E02C File Offset: 0x0000C22C
		public Sql Select(params object[] columns)
		{
			return this.Append(new Sql("SELECT " + string.Join(", ", (from x in columns
			select x.ToString()).ToArray<string>()), new object[0]));
		}

		// Token: 0x06000359 RID: 857 RVA: 0x0000E088 File Offset: 0x0000C288
		public Sql From(params object[] tables)
		{
			return this.Append(new Sql("FROM " + string.Join(", ", (from x in tables
			select x.ToString()).ToArray<string>()), new object[0]));
		}

		// Token: 0x0600035A RID: 858 RVA: 0x0000E0E4 File Offset: 0x0000C2E4
		public Sql GroupBy(params object[] columns)
		{
			return this.Append(new Sql("GROUP BY " + string.Join(", ", (from x in columns
			select x.ToString()).ToArray<string>()), new object[0]));
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0000E140 File Offset: 0x0000C340
		private Sql.SqlJoinClause Join(string joinType, string table)
		{
			return new Sql.SqlJoinClause(this.Append(new Sql(joinType + table, new object[0])));
		}

		// Token: 0x0600035C RID: 860 RVA: 0x0000E15F File Offset: 0x0000C35F
		public Sql.SqlJoinClause InnerJoin(string table)
		{
			return this.Join("INNER JOIN ", table);
		}

		// Token: 0x0600035D RID: 861 RVA: 0x0000E16D File Offset: 0x0000C36D
		public Sql.SqlJoinClause LeftJoin(string table)
		{
			return this.Join("LEFT JOIN ", table);
		}

		// Token: 0x0600035E RID: 862 RVA: 0x0000E17C File Offset: 0x0000C37C
		public static string ProcessParams(string sql, object[] argsSrc, List<object> argsDest)
		{
			return Sql.RxParams.Replace(sql, delegate(Match m)
			{
				string text = m.Value.Substring(1);
				int num;
				object obj;
				if (int.TryParse(text, out num))
				{
					if (num < 0 || num >= argsSrc.Length)
					{
						throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", num, argsSrc.Length, sql));
					}
					obj = argsSrc[num];
				}
				else
				{
					bool flag = false;
					obj = null;
					foreach (object obj2 in argsSrc)
					{
						PropertyInfo property = obj2.GetType().GetProperty(text);
						if (property != null)
						{
							obj = property.GetValue(obj2, null);
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						throw new ArgumentException(string.Concat(new string[]
						{
							"Parameter '@",
							text,
							"' specified but none of the passed arguments have a property with this name (in '",
							sql,
							"')"
						}));
					}
				}
				if (obj is IEnumerable && !(obj is string) && !(obj is byte[]))
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (object item in (obj as IEnumerable))
					{
						stringBuilder.Append(((stringBuilder.Length == 0) ? "@" : ",@") + argsDest.Count.ToString());
						argsDest.Add(item);
					}
					return stringBuilder.ToString();
				}
				argsDest.Add(obj);
				return "@" + (argsDest.Count - 1).ToString();
			});
		}

		// Token: 0x0600035F RID: 863 RVA: 0x0000E1C0 File Offset: 0x0000C3C0
		public static void AddParams(IDbCommand cmd, object[] items, string parameterPrefix)
		{
			foreach (object item in items)
			{
				Sql.AddParam(cmd, item, "@");
			}
		}

		// Token: 0x06000360 RID: 864 RVA: 0x0000E1F0 File Offset: 0x0000C3F0
		public static void AddParam(IDbCommand cmd, object item, string parameterPrefix)
		{
			IDbDataParameter dbDataParameter = item as IDbDataParameter;
			if (dbDataParameter != null)
			{
				dbDataParameter.ParameterName = string.Format("{0}{1}", parameterPrefix, cmd.Parameters.Count);
				cmd.Parameters.Add(dbDataParameter);
				return;
			}
			IDbDataParameter dbDataParameter2 = cmd.CreateParameter();
			dbDataParameter2.ParameterName = string.Format("{0}{1}", parameterPrefix, cmd.Parameters.Count);
			if (item == null)
			{
				dbDataParameter2.Value = DBNull.Value;
			}
			else
			{
				Type type = item.GetType();
				if (type.IsEnum)
				{
					dbDataParameter2.Value = (int)item;
				}
				else if (type == typeof(Guid))
				{
					dbDataParameter2.Value = item.ToString();
					dbDataParameter2.DbType = DbType.String;
					dbDataParameter2.Size = 40;
				}
				else if (type == typeof(string))
				{
					dbDataParameter2.Size = Math.Max(((string)item).Length + 1, 4000);
					dbDataParameter2.Value = item;
				}
				else if (type == typeof(bool))
				{
					dbDataParameter2.Value = (((bool)item) ? 1 : 0);
				}
				else
				{
					dbDataParameter2.Value = item;
				}
			}
			cmd.Parameters.Add(dbDataParameter2);
		}

		// Token: 0x04000137 RID: 311
		private static readonly Regex Filter = new Regex("LOAD\\s*DATA|INTO\\s*(OUTFILE|DUMPFILE)|LOAD_FILE", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		// Token: 0x04000138 RID: 312
		private static readonly Regex RxParams = new Regex("(?<!@)@\\w+", RegexOptions.Compiled);

		// Token: 0x04000139 RID: 313
		private readonly object[] _args;

		// Token: 0x0400013A RID: 314
		private readonly string _sql;

		// Token: 0x0400013B RID: 315
		private object[] _argsFinal;

		// Token: 0x0400013C RID: 316
		private Sql _rhs;

		// Token: 0x0400013D RID: 317
		private string _sqlFinal;

		// Token: 0x020000A2 RID: 162
		public class SqlJoinClause
		{
			// Token: 0x0600048E RID: 1166 RVA: 0x0001157F File Offset: 0x0000F77F
			public SqlJoinClause(Sql sql)
			{
				this._sql = sql;
			}

			// Token: 0x0600048F RID: 1167 RVA: 0x0001158E File Offset: 0x0000F78E
			public Sql On(string onClause, params object[] args)
			{
				return this._sql.Append("ON " + onClause, args);
			}

			// Token: 0x0400021F RID: 543
			private readonly Sql _sql;
		}
	}
}
