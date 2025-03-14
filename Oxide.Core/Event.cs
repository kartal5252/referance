using System;
using System.Collections.Generic;

namespace Oxide.Core
{
	// Token: 0x02000007 RID: 7
	public class Event
	{
		// Token: 0x0600001C RID: 28 RVA: 0x0000292C File Offset: 0x00000B2C
		public static void Remove(ref Event.Callback callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x0000293D File Offset: 0x00000B3D
		public static void Remove<T1>(ref Event.Callback<T1> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x0000294E File Offset: 0x00000B4E
		public static void Remove<T1, T2>(ref Event.Callback<T1, T2> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		// Token: 0x0600001F RID: 31 RVA: 0x0000295F File Offset: 0x00000B5F
		public static void Remove<T1, T2, T3>(ref Event.Callback<T1, T2, T3> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002970 File Offset: 0x00000B70
		public static void Remove<T1, T2, T3, T4>(ref Event.Callback<T1, T2, T3, T4> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002981 File Offset: 0x00000B81
		public static void Remove<T1, T2, T3, T4, T5>(ref Event.Callback<T1, T2, T3, T4, T5> callback)
		{
			if (callback == null)
			{
				return;
			}
			callback.Remove();
			callback = null;
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00002994 File Offset: 0x00000B94
		public void Add(Event.Callback callback)
		{
			callback.Handler = this;
			object @lock = this.Lock;
			lock (@lock)
			{
				Event.Callback last = this.Last;
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

		// Token: 0x06000023 RID: 35 RVA: 0x00002A00 File Offset: 0x00000C00
		public Event.Callback Add(Action callback)
		{
			Event.Callback callback2 = new Event.Callback(callback);
			this.Add(callback2);
			return callback2;
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002A1C File Offset: 0x00000C1C
		public void Invoke()
		{
			object @lock = this.Lock;
			lock (@lock)
			{
				this.Invoking = true;
				for (Event.Callback callback = this.First; callback != null; callback = callback.Next)
				{
					callback.Call();
				}
				this.Invoking = false;
				Queue<Event.Callback> removedQueue = this.RemovedQueue;
				while (removedQueue.Count > 0)
				{
					Event.Callback callback = removedQueue.Dequeue();
					callback.Previous = null;
					callback.Next = null;
				}
			}
		}

		// Token: 0x04000010 RID: 16
		public Event.Callback First;

		// Token: 0x04000011 RID: 17
		public Event.Callback Last;

		// Token: 0x04000012 RID: 18
		internal object Lock = new object();

		// Token: 0x04000013 RID: 19
		internal bool Invoking;

		// Token: 0x04000014 RID: 20
		internal Queue<Event.Callback> RemovedQueue = new Queue<Event.Callback>();

		// Token: 0x02000061 RID: 97
		// (Invoke) Token: 0x0600039B RID: 923
		public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4);

		// Token: 0x02000062 RID: 98
		public class Callback
		{
			// Token: 0x0600039E RID: 926 RVA: 0x0000EF24 File Offset: 0x0000D124
			public Callback(Action callback)
			{
				this.Invoke = callback;
			}

			// Token: 0x0600039F RID: 927 RVA: 0x0000EF34 File Offset: 0x0000D134
			public void Call()
			{
				Action invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke();
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			// Token: 0x060003A0 RID: 928 RVA: 0x0000EF78 File Offset: 0x0000D178
			public void Remove()
			{
				Event handler = this.Handler;
				Event.Callback next = this.Next;
				Event.Callback previous = this.Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					this.Previous = null;
					this.Next = null;
				}
				this.Invoke = null;
				this.Handler = null;
			}

			// Token: 0x0400014E RID: 334
			public Action Invoke;

			// Token: 0x0400014F RID: 335
			internal Event.Callback Previous;

			// Token: 0x04000150 RID: 336
			internal Event.Callback Next;

			// Token: 0x04000151 RID: 337
			internal Event Handler;
		}

		// Token: 0x02000063 RID: 99
		public class Callback<T>
		{
			// Token: 0x060003A1 RID: 929 RVA: 0x0000F006 File Offset: 0x0000D206
			public Callback(Action<T> callback)
			{
				this.Invoke = callback;
			}

			// Token: 0x060003A2 RID: 930 RVA: 0x0000F018 File Offset: 0x0000D218
			public void Call(T arg0)
			{
				Action<T> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			// Token: 0x060003A3 RID: 931 RVA: 0x0000F060 File Offset: 0x0000D260
			public void Remove()
			{
				Event<T> handler = this.Handler;
				Event.Callback<T> next = this.Next;
				Event.Callback<T> previous = this.Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					this.Previous = null;
					this.Next = null;
				}
				this.Invoke = null;
				this.Handler = null;
			}

			// Token: 0x04000152 RID: 338
			public Action<T> Invoke;

			// Token: 0x04000153 RID: 339
			internal Event.Callback<T> Previous;

			// Token: 0x04000154 RID: 340
			internal Event.Callback<T> Next;

			// Token: 0x04000155 RID: 341
			internal Event<T> Handler;
		}

		// Token: 0x02000064 RID: 100
		public class Callback<T1, T2>
		{
			// Token: 0x060003A4 RID: 932 RVA: 0x0000F0EE File Offset: 0x0000D2EE
			public Callback(Action<T1, T2> callback)
			{
				this.Invoke = callback;
			}

			// Token: 0x060003A5 RID: 933 RVA: 0x0000F100 File Offset: 0x0000D300
			public void Call(T1 arg0, T2 arg1)
			{
				Action<T1, T2> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			// Token: 0x060003A6 RID: 934 RVA: 0x0000F148 File Offset: 0x0000D348
			public void Remove()
			{
				Event<T1, T2> handler = this.Handler;
				Event.Callback<T1, T2> next = this.Next;
				Event.Callback<T1, T2> previous = this.Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					this.Previous = null;
					this.Next = null;
				}
				this.Invoke = null;
				this.Handler = null;
			}

			// Token: 0x04000156 RID: 342
			public Action<T1, T2> Invoke;

			// Token: 0x04000157 RID: 343
			internal Event.Callback<T1, T2> Previous;

			// Token: 0x04000158 RID: 344
			internal Event.Callback<T1, T2> Next;

			// Token: 0x04000159 RID: 345
			internal Event<T1, T2> Handler;
		}

		// Token: 0x02000065 RID: 101
		public class Callback<T1, T2, T3>
		{
			// Token: 0x060003A7 RID: 935 RVA: 0x0000F1D6 File Offset: 0x0000D3D6
			public Callback(Action<T1, T2, T3> callback)
			{
				this.Invoke = callback;
			}

			// Token: 0x060003A8 RID: 936 RVA: 0x0000F1E8 File Offset: 0x0000D3E8
			public void Call(T1 arg0, T2 arg1, T3 arg2)
			{
				Action<T1, T2, T3> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1, arg2);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			// Token: 0x060003A9 RID: 937 RVA: 0x0000F230 File Offset: 0x0000D430
			public void Remove()
			{
				Event<T1, T2, T3> handler = this.Handler;
				Event.Callback<T1, T2, T3> next = this.Next;
				Event.Callback<T1, T2, T3> previous = this.Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					this.Previous = null;
					this.Next = null;
				}
				this.Invoke = null;
				this.Handler = null;
			}

			// Token: 0x0400015A RID: 346
			public Action<T1, T2, T3> Invoke;

			// Token: 0x0400015B RID: 347
			internal Event.Callback<T1, T2, T3> Previous;

			// Token: 0x0400015C RID: 348
			internal Event.Callback<T1, T2, T3> Next;

			// Token: 0x0400015D RID: 349
			internal Event<T1, T2, T3> Handler;
		}

		// Token: 0x02000066 RID: 102
		public class Callback<T1, T2, T3, T4>
		{
			// Token: 0x060003AA RID: 938 RVA: 0x0000F2BE File Offset: 0x0000D4BE
			public Callback(Action<T1, T2, T3, T4> callback)
			{
				this.Invoke = callback;
			}

			// Token: 0x060003AB RID: 939 RVA: 0x0000F2D0 File Offset: 0x0000D4D0
			public void Call(T1 arg0, T2 arg1, T3 arg2, T4 arg3)
			{
				Action<T1, T2, T3, T4> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1, arg2, arg3);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			// Token: 0x060003AC RID: 940 RVA: 0x0000F31C File Offset: 0x0000D51C
			public void Remove()
			{
				Event<T1, T2, T3, T4> handler = this.Handler;
				Event.Callback<T1, T2, T3, T4> next = this.Next;
				Event.Callback<T1, T2, T3, T4> previous = this.Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					this.Previous = null;
					this.Next = null;
				}
				this.Invoke = null;
				this.Handler = null;
			}

			// Token: 0x0400015E RID: 350
			public Action<T1, T2, T3, T4> Invoke;

			// Token: 0x0400015F RID: 351
			internal Event.Callback<T1, T2, T3, T4> Previous;

			// Token: 0x04000160 RID: 352
			internal Event.Callback<T1, T2, T3, T4> Next;

			// Token: 0x04000161 RID: 353
			internal Event<T1, T2, T3, T4> Handler;
		}

		// Token: 0x02000067 RID: 103
		public class Callback<T1, T2, T3, T4, T5>
		{
			// Token: 0x060003AD RID: 941 RVA: 0x0000F3AA File Offset: 0x0000D5AA
			public Callback(Event.Action<T1, T2, T3, T4, T5> callback)
			{
				this.Invoke = callback;
			}

			// Token: 0x060003AE RID: 942 RVA: 0x0000F3BC File Offset: 0x0000D5BC
			public void Call(T1 arg0, T2 arg1, T3 arg2, T4 arg3, T5 arg4)
			{
				Event.Action<T1, T2, T3, T4, T5> invoke = this.Invoke;
				if (invoke == null)
				{
					return;
				}
				try
				{
					invoke(arg0, arg1, arg2, arg3, arg4);
				}
				catch (Exception ex)
				{
					Interface.Oxide.LogException("Exception while invoking event handler", ex);
				}
			}

			// Token: 0x060003AF RID: 943 RVA: 0x0000F408 File Offset: 0x0000D608
			public void Remove()
			{
				Event<T1, T2, T3, T4, T5> handler = this.Handler;
				Event.Callback<T1, T2, T3, T4, T5> next = this.Next;
				Event.Callback<T1, T2, T3, T4, T5> previous = this.Previous;
				if (previous == null)
				{
					handler.First = next;
				}
				else
				{
					previous.Next = next;
					if (next == null)
					{
						handler.Last = previous;
					}
				}
				if (next == null)
				{
					handler.Last = previous;
				}
				else
				{
					next.Previous = previous;
					if (previous == null)
					{
						handler.First = next;
					}
				}
				if (handler.Invoking)
				{
					handler.RemovedQueue.Enqueue(this);
				}
				else
				{
					this.Previous = null;
					this.Next = null;
				}
				this.Invoke = null;
				this.Handler = null;
			}

			// Token: 0x04000162 RID: 354
			public Event.Action<T1, T2, T3, T4, T5> Invoke;

			// Token: 0x04000163 RID: 355
			internal Event.Callback<T1, T2, T3, T4, T5> Previous;

			// Token: 0x04000164 RID: 356
			internal Event.Callback<T1, T2, T3, T4, T5> Next;

			// Token: 0x04000165 RID: 357
			internal Event<T1, T2, T3, T4, T5> Handler;
		}
	}
}
