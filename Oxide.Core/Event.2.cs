using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x02000008 RID: 8
	public class Event<T>
	{
		// Token: 0x06000026 RID: 38 RVA: 0x00002AC0 File Offset: 0x00000CC0
		public void Add(Event.Callback<T> callback)
		{
			callback.Handler = this;
			object @lock = this.Lock;
			lock (@lock)
			{
				Event.Callback<T> last = this.Last;
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

		// Token: 0x06000027 RID: 39 RVA: 0x00002B2C File Offset: 0x00000D2C
		public Event.Callback<T> Add(Action<T> callback)
		{
			Event.Callback<T> callback2 = new Event.Callback<T>(callback);
			this.Add(callback2);
			return callback2;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00002B48 File Offset: 0x00000D48
		public void Invoke(T arg0)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0);
				}
				this.Invoking = false;
				Queue<Event.Callback<T>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x04000015 RID: 21
		public Event.Callback<T> First;

		// Token: 0x04000016 RID: 22
		public Event.Callback<T> Last;

		// Token: 0x04000017 RID: 23
		internal object Lock = new object();

		// Token: 0x04000018 RID: 24
		internal bool Invoking;

		// Token: 0x04000019 RID: 25
		internal Queue<Event.Callback<T>> RemovedQueue = new Queue<Event.Callback<T>>();
	}
}
