using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Oxide.Core
{
	// Token: 0x02000016 RID: 22
	public class ConcurrentHashSet<T> : ICollection<T>, IEnumerable<T>, IEnumerable
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x00005E95 File Offset: 0x00004095
		public ConcurrentHashSet()
		{
			this.collection = new HashSet<T>();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00005EB3 File Offset: 0x000040B3
		public ConcurrentHashSet(ICollection<T> values)
		{
			this.collection = new HashSet<T>(values);
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x00005ED2 File Offset: 0x000040D2
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00005ED8 File Offset: 0x000040D8
		public int Count
		{
			get
			{
				object obj = this.syncRoot;
				int count;
				lock (obj)
				{
					count = this.collection.Count;
				}
				return count;
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x00005F18 File Offset: 0x00004118
		public bool Contains(T value)
		{
			object obj = this.syncRoot;
			bool result;
			lock (obj)
			{
				result = this.collection.Contains(value);
			}
			return result;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00005F5C File Offset: 0x0000415C
		public bool Add(T value)
		{
			object obj = this.syncRoot;
			bool result;
			lock (obj)
			{
				result = this.collection.Add(value);
			}
			return result;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00005FA0 File Offset: 0x000041A0
		public bool Remove(T value)
		{
			object obj = this.syncRoot;
			bool result;
			lock (obj)
			{
				result = this.collection.Remove(value);
			}
			return result;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005FE4 File Offset: 0x000041E4
		public void Clear()
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				this.collection.Clear();
			}
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006024 File Offset: 0x00004224
		public void CopyTo(T[] array, int index)
		{
			object obj = this.syncRoot;
			lock (obj)
			{
				this.collection.CopyTo(array, index);
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00006064 File Offset: 0x00004264
		public IEnumerator<T> GetEnumerator()
		{
			return this.collection.GetEnumerator();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006078 File Offset: 0x00004278
		public bool Any(Func<T, bool> callback)
		{
			object obj = this.syncRoot;
			bool result;
			lock (obj)
			{
				result = this.collection.Any(callback);
			}
			return result;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x000060BC File Offset: 0x000042BC
		public T[] ToArray()
		{
			object obj = this.syncRoot;
			T[] result;
			lock (obj)
			{
				result = this.collection.ToArray<T>();
			}
			return result;
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x000060FC File Offset: 0x000042FC
		public bool TryDequeue(out T value)
		{
			object obj = this.syncRoot;
			bool result;
			lock (obj)
			{
				value = this.collection.ElementAtOrDefault(0);
				if (value != null)
				{
					this.collection.Remove(value);
				}
				result = (value != null);
			}
			return result;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00006170 File Offset: 0x00004370
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00006178 File Offset: 0x00004378
		void ICollection<!0>.Add(T value)
		{
			this.Add(value);
		}

		// Token: 0x0400005F RID: 95
		private readonly HashSet<T> collection;

		// Token: 0x04000060 RID: 96
		private readonly object syncRoot = new object();
	}
}
