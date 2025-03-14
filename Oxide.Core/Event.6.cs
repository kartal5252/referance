using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x0200000C RID: 12
	public class Event<T1, T2, T3, T4, T5>
	{
		// Token: 0x0600003F RID: 63 RVA: 0x000034CC File Offset: 0x000016CC
		public void Add(Event.Callback<T1, T2, T3, T4, T5> callback)
		{
			callback.Handler = this;
			object @lock = this.Lock;
			lock (@lock)
			{
				Event.Callback<T1, T2, T3, T4, T5> last = this.Last;
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

		// Token: 0x06000040 RID: 64 RVA: 0x00003538 File Offset: 0x00001738
		public Event.Callback<T1, T2, T3, T4, T5> Add(Event.Action<T1, T2, T3, T4, T5> callback)
		{
			Event.Callback<T1, T2, T3, T4, T5> callback2 = new Event.Callback<T1, T2, T3, T4, T5>(callback);
			this.Add(callback2);
			return callback2;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00003554 File Offset: 0x00001754
		public void Invoke()
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(default(T1), default(T2), default(T3), default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00003608 File Offset: 0x00001808
		public void Invoke(T1 arg0)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, default(T2), default(T3), default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x000036B4 File Offset: 0x000018B4
		public void Invoke(T1 arg0, T2 arg1)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, default(T3), default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00003754 File Offset: 0x00001954
		public void Invoke(T1 arg0, T2 arg1, T3 arg2)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, default(T4), default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000037EC File Offset: 0x000019EC
		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, arg3, default(T5));
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x0000387C File Offset: 0x00001A7C
		public void Invoke(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback<T1, T2, T3, T4, T5> callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call(arg0, arg1, arg2, arg3, arg4);
				}
				this.Invoking = false;
				Queue<Event.Callback<T1, T2, T3, T4, T5>> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback<T1, T2, T3, T4, T5> callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x04000029 RID: 41
		public Event.Callback<T1, T2, T3, T4, T5> First;

		// Token: 0x0400002A RID: 42
		public Event.Callback<T1, T2, T3, T4, T5> Last;

		// Token: 0x0400002B RID: 43
		internal object Lock = new object();

		// Token: 0x0400002C RID: 44
		internal bool Invoking;

		// Token: 0x0400002D RID: 45
		internal Queue<Event.Callback<T1, T2, T3, T4, T5>> RemovedQueue = new Queue<Event.Callback<T1, T2, T3, T4, T5>>();
	}
}
