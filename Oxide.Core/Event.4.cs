using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x0200000A RID: 10
	public class Event<T1, T2, T3>
	{
		// Token: 0x06000030 RID: 48 RVA: 0x00002E38 File Offset: 0x00001038
		public void Add(Event.Callback<T1, T2, T3> callback)
		{
			callback.Handler = this;
			object @lock = this.Lock;
			lock (@lock)
			{
				Event.Callback<T1, T2, T3> last = this.Last;
				if (last == null)
				{
					this.First = callback;
					this.Last = callback;
				}
				else
				{
					last.Next = callback;
					callback.Previous = last;
					this.Last = callback;
				}
			}
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002EA4 File Offset: 0x000010A4
		public Event.Callback<T1, T2, T3> Add(Action<T1, T2, T3> callback)
		{
			Event.Callback<T1, T2, T3> callback2 = new Event.Callback<T1, T2, T3>(callback);
			this.Add(callback2);
			return callback2;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002EC0 File Offset: 0x000010C0
		public void Invoke()
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Invoke(default(T1), default(T2), default(T3));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002F64 File Offset: 0x00001164
		public void Invoke(T1 arg0)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2), default(T3));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002FFC File Offset: 0x000011FC
		public void Invoke(T1 arg0, T2 arg1)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, default(T3));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003088 File Offset: 0x00001288
		public void Invoke(T1 arg0, T2 arg1, T3 arg2)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2);
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0400001F RID: 31
		public Event.Callback<T1, T2, T3> First;

		// Token: 0x04000020 RID: 32
		public Event.Callback<T1, T2, T3> Last;

		// Token: 0x04000021 RID: 33
		internal object Lock = new object();

		// Token: 0x04000022 RID: 34
		internal bool Invoking;

		// Token: 0x04000023 RID: 35
		internal Queue<Event.Callback<T1, T2, T3>> RemovedQueue = new Queue<Event.Callback<T1, T2, T3>>();
	}
}
