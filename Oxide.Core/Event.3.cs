using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x02000009 RID: 9
	public class Event<T1, T2>
	{
		// Token: 0x0600002A RID: 42 RVA: 0x00002BEC File Offset: 0x00000DEC
		public void Add(Event.Callback<T1, T2> callback)
		{
			callback.Handler = this;
			object @lock = this.Lock;
			lock (@lock)
			{
				Event.Callback<T1, T2> last = this.Last;
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

		// Token: 0x0600002B RID: 43 RVA: 0x00002C58 File Offset: 0x00000E58
		public Event.Callback<T1, T2> Add(Action<T1, T2> callback)
		{
			Event.Callback<T1, T2> callback2 = new Event.Callback<T1, T2>(callback);
			this.Add(callback2);
			return callback2;
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002C74 File Offset: 0x00000E74
		public void Invoke()
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(default(T1), default(T2));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0600002D RID: 45 RVA: 0x00002D08 File Offset: 0x00000F08
		public void Invoke(T1 arg0)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0600002E RID: 46 RVA: 0x00002D94 File Offset: 0x00000F94
		public void Invoke(T1 arg0, T2 arg1)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1);
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0400001A RID: 26
		public Event.Callback<T1, T2> First;

		// Token: 0x0400001B RID: 27
		public Event.Callback<T1, T2> Last;

		// Token: 0x0400001C RID: 28
		internal object Lock = new object();

		// Token: 0x0400001D RID: 29
		internal bool Invoking;

		// Token: 0x0400001E RID: 30
		internal Queue<Event.Callback<T1, T2>> RemovedQueue = new Queue<Event.Callback<T1, T2>>();
	}
}
