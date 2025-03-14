using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x0200000B RID: 11
	public class Event<T1, T2, T3, T4>
	{
		// Token: 0x06000037 RID: 55 RVA: 0x0000312C File Offset: 0x0000132C
		public void Add(Event.Callback<T1, T2, T3, T4> callback)
		{
			callback.Handler = this;
			object @lock = this.Lock;
			lock (@lock)
			{
				Event.Callback<T1, T2, T3, T4> last = this.Last;
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

		// Token: 0x06000038 RID: 56 RVA: 0x00003198 File Offset: 0x00001398
		public Event.Callback<T1, T2, T3, T4> Add(Action<T1, T2, T3, T4> callback)
		{
			Event.Callback<T1, T2, T3, T4> callback2 = new Event.Callback<T1, T2, T3, T4>(callback);
			this.Add(callback2);
			return callback2;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000031B4 File Offset: 0x000013B4
		public void Invoke()
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(default(T1), default(T2), default(T3), default(T4));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x0000325C File Offset: 0x0000145C
		public void Invoke(T1 arg0)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2), default(T3), default(T4));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x000032FC File Offset: 0x000014FC
		public void Invoke(T1 arg0, T2 arg1)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, default(T3), default(T4));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003394 File Offset: 0x00001594
		public void Invoke(T1 arg0, T2 arg1, T3 arg2)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, default(T4));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003424 File Offset: 0x00001624
		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, arg3);
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x04000024 RID: 36
		public Event.Callback<T1, T2, T3, T4> First;

		// Token: 0x04000025 RID: 37
		public Event.Callback<T1, T2, T3, T4> Last;

		// Token: 0x04000026 RID: 38
		internal object Lock = new object();

		// Token: 0x04000027 RID: 39
		internal bool Invoking;

		// Token: 0x04000028 RID: 40
		internal Queue<Event.Callback<T1, T2, T3, T4>> RemovedQueue = new Queue<Event.Callback<T1, T2, T3, T4>>();
	}
}
