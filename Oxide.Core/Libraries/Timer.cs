using System;
using System.Collections.Generic;
using Oxide.Core.Plugins;

namespace Oxide.Core.Libraries
{
	// Token: 0x0200003F RID: 63
	public class Timer : Library
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x0600026C RID: 620 RVA: 0x0000BE6D File Offset: 0x0000A06D
		public override bool IsGlobal
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x0600026D RID: 621 RVA: 0x0000BE70 File Offset: 0x0000A070
		// (set) Token: 0x0600026E RID: 622 RVA: 0x0000BE77 File Offset: 0x0000A077
		public static int Count { get; private set; }

		// Token: 0x0600026F RID: 623 RVA: 0x0000BE80 File Offset: 0x0000A080
		public Timer()
		{
			for (int i = 0; i < 512; i++)
			{
				this.timeSlots[i] = new Timer.TimeSlot();
			}
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000BEDC File Offset: 0x0000A0DC
		public void Update(float delta)
		{
			float now = Timer.Oxide.Now;
			Timer.TimeSlot[] array = this.timeSlots;
			Queue<Timer.TimerInstance> queue = this.expiredInstanceQueue;
			int num = 0;
			object @lock = Timer.Lock;
			lock (@lock)
			{
				int num2 = this.currentSlot;
				double num3 = this.nextSlotAt;
				for (;;)
				{
					array[num2].GetExpired((num3 > (double)now) ? ((double)now) : num3, queue);
					if ((double)now <= num3)
					{
						break;
					}
					num++;
					num2 = ((num2 < 511) ? (num2 + 1) : 0);
					num3 += 0.009999999776482582;
				}
				if (num > 0)
				{
					this.currentSlot = num2;
					this.nextSlotAt = num3;
				}
				int count = queue.Count;
				for (int i = 0; i < count; i++)
				{
					Timer.TimerInstance timerInstance = queue.Dequeue();
					if (!timerInstance.Destroyed)
					{
						timerInstance.Invoke(now);
					}
				}
			}
		}

		// Token: 0x06000271 RID: 625 RVA: 0x0000BFC4 File Offset: 0x0000A1C4
		internal Timer.TimerInstance AddTimer(int repetitions, float delay, Action callback, Plugin owner = null)
		{
			object @lock = Timer.Lock;
			Timer.TimerInstance result;
			lock (@lock)
			{
				Queue<Timer.TimerInstance> pool = Timer.TimerInstance.Pool;
				Timer.TimerInstance timerInstance;
				if (pool.Count > 0)
				{
					timerInstance = pool.Dequeue();
					timerInstance.Load(this, repetitions, delay, callback, owner);
				}
				else
				{
					timerInstance = new Timer.TimerInstance(this, repetitions, delay, callback, owner);
				}
				this.InsertTimer(timerInstance, timerInstance.ExpiresAt < Timer.Oxide.Now);
				result = timerInstance;
			}
			return result;
		}

		// Token: 0x06000272 RID: 626 RVA: 0x0000C044 File Offset: 0x0000A244
		private void InsertTimer(Timer.TimerInstance timer, bool in_past = false)
		{
			int num = in_past ? this.currentSlot : ((int)(timer.ExpiresAt / 0.01f) & 511);
			this.timeSlots[num].InsertTimer(timer);
		}

		// Token: 0x06000273 RID: 627 RVA: 0x0000C07E File Offset: 0x0000A27E
		[LibraryFunction("Once")]
		public Timer.TimerInstance Once(float delay, Action callback, Plugin owner = null)
		{
			return this.AddTimer(1, delay, callback, owner);
		}

		// Token: 0x06000274 RID: 628 RVA: 0x0000C08A File Offset: 0x0000A28A
		[LibraryFunction("Repeat")]
		public Timer.TimerInstance Repeat(float delay, int reps, Action callback, Plugin owner = null)
		{
			return this.AddTimer(reps, delay, callback, owner);
		}

		// Token: 0x06000275 RID: 629 RVA: 0x0000C097 File Offset: 0x0000A297
		[LibraryFunction("NextFrame")]
		public Timer.TimerInstance NextFrame(Action callback)
		{
			return this.AddTimer(1, 0f, callback, null);
		}

		// Token: 0x040000EB RID: 235
		internal static readonly object Lock = new object();

		// Token: 0x040000EC RID: 236
		internal static readonly OxideMod Oxide = Interface.Oxide;

		// Token: 0x040000ED RID: 237
		public const int TimeSlots = 512;

		// Token: 0x040000EE RID: 238
		public const int LastTimeSlot = 511;

		// Token: 0x040000EF RID: 239
		public const float TickDuration = 0.01f;

		// Token: 0x040000F0 RID: 240
		private readonly Timer.TimeSlot[] timeSlots = new Timer.TimeSlot[512];

		// Token: 0x040000F1 RID: 241
		private readonly Queue<Timer.TimerInstance> expiredInstanceQueue = new Queue<Timer.TimerInstance>();

		// Token: 0x040000F2 RID: 242
		private int currentSlot;

		// Token: 0x040000F3 RID: 243
		private double nextSlotAt = 0.009999999776482582;

		// Token: 0x0200008F RID: 143
		public class TimeSlot
		{
			// Token: 0x0600042A RID: 1066 RVA: 0x000102D4 File Offset: 0x0000E4D4
			public void GetExpired(double now, Queue<Timer.TimerInstance> queue)
			{
				Timer.TimerInstance timerInstance = this.FirstInstance;
				while (timerInstance != null && (double)timerInstance.ExpiresAt <= now)
				{
					queue.Enqueue(timerInstance);
					timerInstance = timerInstance.NextInstance;
				}
			}

			// Token: 0x0600042B RID: 1067 RVA: 0x00010308 File Offset: 0x0000E508
			public void InsertTimer(Timer.TimerInstance timer)
			{
				float expiresAt = timer.ExpiresAt;
				Timer.TimerInstance firstInstance = this.FirstInstance;
				Timer.TimerInstance lastInstance = this.LastInstance;
				Timer.TimerInstance timerInstance = firstInstance;
				if (firstInstance != null)
				{
					float expiresAt2 = firstInstance.ExpiresAt;
					float expiresAt3 = lastInstance.ExpiresAt;
					if (expiresAt <= expiresAt2)
					{
						timerInstance = firstInstance;
					}
					else if (expiresAt >= expiresAt3)
					{
						timerInstance = null;
					}
					else if (expiresAt3 - expiresAt < expiresAt - expiresAt2)
					{
						timerInstance = lastInstance;
						for (Timer.TimerInstance timerInstance2 = timerInstance; timerInstance2 != null; timerInstance2 = timerInstance2.PreviousInstance)
						{
							if (timerInstance2.ExpiresAt <= expiresAt)
							{
								break;
							}
							timerInstance = timerInstance2;
						}
					}
					else
					{
						while (timerInstance != null && timerInstance.ExpiresAt <= expiresAt)
						{
							timerInstance = timerInstance.NextInstance;
						}
					}
				}
				if (timerInstance == null)
				{
					timer.NextInstance = null;
					if (lastInstance == null)
					{
						this.FirstInstance = timer;
						this.LastInstance = timer;
					}
					else
					{
						lastInstance.NextInstance = timer;
						timer.PreviousInstance = lastInstance;
						this.LastInstance = timer;
					}
				}
				else
				{
					Timer.TimerInstance previousInstance = timerInstance.PreviousInstance;
					if (previousInstance == null)
					{
						this.FirstInstance = timer;
					}
					else
					{
						previousInstance.NextInstance = timer;
					}
					timerInstance.PreviousInstance = timer;
					timer.PreviousInstance = previousInstance;
					timer.NextInstance = timerInstance;
				}
				timer.Added(this);
			}

			// Token: 0x040001D6 RID: 470
			public int Count;

			// Token: 0x040001D7 RID: 471
			public Timer.TimerInstance FirstInstance;

			// Token: 0x040001D8 RID: 472
			public Timer.TimerInstance LastInstance;
		}

		// Token: 0x02000090 RID: 144
		public class TimerInstance
		{
			// Token: 0x170000A8 RID: 168
			// (get) Token: 0x0600042D RID: 1069 RVA: 0x00010407 File Offset: 0x0000E607
			// (set) Token: 0x0600042E RID: 1070 RVA: 0x0001040F File Offset: 0x0000E60F
			public int Repetitions { get; private set; }

			// Token: 0x170000A9 RID: 169
			// (get) Token: 0x0600042F RID: 1071 RVA: 0x00010418 File Offset: 0x0000E618
			// (set) Token: 0x06000430 RID: 1072 RVA: 0x00010420 File Offset: 0x0000E620
			public float Delay { get; private set; }

			// Token: 0x170000AA RID: 170
			// (get) Token: 0x06000431 RID: 1073 RVA: 0x00010429 File Offset: 0x0000E629
			// (set) Token: 0x06000432 RID: 1074 RVA: 0x00010431 File Offset: 0x0000E631
			public Action Callback { get; private set; }

			// Token: 0x170000AB RID: 171
			// (get) Token: 0x06000433 RID: 1075 RVA: 0x0001043A File Offset: 0x0000E63A
			// (set) Token: 0x06000434 RID: 1076 RVA: 0x00010442 File Offset: 0x0000E642
			public bool Destroyed { get; private set; }

			// Token: 0x170000AC RID: 172
			// (get) Token: 0x06000435 RID: 1077 RVA: 0x0001044B File Offset: 0x0000E64B
			// (set) Token: 0x06000436 RID: 1078 RVA: 0x00010453 File Offset: 0x0000E653
			public Plugin Owner { get; private set; }

			// Token: 0x06000437 RID: 1079 RVA: 0x0001045C File Offset: 0x0000E65C
			internal TimerInstance(Timer timer, int repetitions, float delay, Action callback, Plugin owner)
			{
				this.Load(timer, repetitions, delay, callback, owner);
			}

			// Token: 0x06000438 RID: 1080 RVA: 0x00010474 File Offset: 0x0000E674
			internal void Load(Timer timer, int repetitions, float delay, Action callback, Plugin owner)
			{
				this.timer = timer;
				this.Repetitions = repetitions;
				this.Delay = delay;
				this.Callback = callback;
				this.ExpiresAt = Timer.Oxide.Now + delay;
				this.Owner = owner;
				this.Destroyed = false;
				if (owner != null)
				{
					this.removedFromManager = owner.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.OnRemovedFromManager));
				}
			}

			// Token: 0x06000439 RID: 1081 RVA: 0x000104E4 File Offset: 0x0000E6E4
			public void Reset(float delay = -1f, int repetitions = 1)
			{
				object @lock = Timer.Lock;
				lock (@lock)
				{
					if (delay < 0f)
					{
						delay = this.Delay;
					}
					else
					{
						this.Delay = delay;
					}
					this.Repetitions = repetitions;
					this.ExpiresAt = Timer.Oxide.Now + delay;
					if (this.Destroyed)
					{
						this.Destroyed = false;
						Plugin owner = this.Owner;
						if (owner != null)
						{
							this.removedFromManager = owner.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.OnRemovedFromManager));
						}
					}
					else
					{
						this.Remove();
					}
					this.timer.InsertTimer(this, false);
				}
			}

			// Token: 0x0600043A RID: 1082 RVA: 0x00010594 File Offset: 0x0000E794
			public bool Destroy()
			{
				object @lock = Timer.Lock;
				lock (@lock)
				{
					if (this.Destroyed)
					{
						return false;
					}
					this.Destroyed = true;
					this.Remove();
					Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
				}
				return true;
			}

			// Token: 0x0600043B RID: 1083 RVA: 0x000105F0 File Offset: 0x0000E7F0
			public bool DestroyToPool()
			{
				object @lock = Timer.Lock;
				lock (@lock)
				{
					if (this.Destroyed)
					{
						return false;
					}
					this.Destroyed = true;
					this.Callback = null;
					this.Remove();
					Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
					Queue<Timer.TimerInstance> pool = Timer.TimerInstance.Pool;
					if (pool.Count < 5000)
					{
						pool.Enqueue(this);
					}
				}
				return true;
			}

			// Token: 0x0600043C RID: 1084 RVA: 0x0001066C File Offset: 0x0000E86C
			internal void Added(Timer.TimeSlot time_slot)
			{
				time_slot.Count++;
				Timer.Count++;
				this.TimeSlot = time_slot;
			}

			// Token: 0x0600043D RID: 1085 RVA: 0x00010690 File Offset: 0x0000E890
			internal void Invoke(float now)
			{
				if (this.Repetitions > 0)
				{
					int num = this.Repetitions - 1;
					this.Repetitions = num;
					if (num == 0)
					{
						this.Destroy();
						this.FireCallback();
						return;
					}
				}
				this.Remove();
				float num2 = this.ExpiresAt + this.Delay;
				this.ExpiresAt = num2;
				this.timer.InsertTimer(this, num2 < now);
				this.FireCallback();
			}

			// Token: 0x0600043E RID: 1086 RVA: 0x000106F8 File Offset: 0x0000E8F8
			internal void Remove()
			{
				Timer.TimeSlot timeSlot = this.TimeSlot;
				if (timeSlot == null)
				{
					return;
				}
				timeSlot.Count--;
				Timer.Count--;
				Timer.TimerInstance previousInstance = this.PreviousInstance;
				Timer.TimerInstance nextInstance = this.NextInstance;
				if (nextInstance == null)
				{
					timeSlot.LastInstance = previousInstance;
				}
				else
				{
					nextInstance.PreviousInstance = previousInstance;
				}
				if (previousInstance == null)
				{
					timeSlot.FirstInstance = nextInstance;
				}
				else
				{
					previousInstance.NextInstance = nextInstance;
				}
				this.TimeSlot = null;
				this.PreviousInstance = null;
				this.NextInstance = null;
			}

			// Token: 0x0600043F RID: 1087 RVA: 0x00010774 File Offset: 0x0000E974
			private void FireCallback()
			{
				Plugin owner = this.Owner;
				if (owner != null)
				{
					owner.TrackStart();
				}
				try
				{
					this.Callback();
				}
				catch (Exception ex)
				{
					this.Destroy();
					string text = string.Format("Failed to run a {0:0.00} timer", this.Delay);
					if (this.Owner && this.Owner != null)
					{
						text += string.Format(" in '{0} v{1}'", this.Owner.Name, this.Owner.Version);
					}
					Interface.Oxide.LogException(text, ex);
				}
				finally
				{
					Plugin owner2 = this.Owner;
					if (owner2 != null)
					{
						owner2.TrackEnd();
					}
				}
			}

			// Token: 0x06000440 RID: 1088 RVA: 0x0001083C File Offset: 0x0000EA3C
			private void OnRemovedFromManager(Plugin sender, PluginManager manager)
			{
				this.Destroy();
			}

			// Token: 0x040001D9 RID: 473
			public const int MaxPooled = 5000;

			// Token: 0x040001DA RID: 474
			internal static Queue<Timer.TimerInstance> Pool = new Queue<Timer.TimerInstance>();

			// Token: 0x040001E0 RID: 480
			internal float ExpiresAt;

			// Token: 0x040001E1 RID: 481
			internal Timer.TimeSlot TimeSlot;

			// Token: 0x040001E2 RID: 482
			internal Timer.TimerInstance NextInstance;

			// Token: 0x040001E3 RID: 483
			internal Timer.TimerInstance PreviousInstance;

			// Token: 0x040001E4 RID: 484
			private Event.Callback<Plugin, PluginManager> removedFromManager;

			// Token: 0x040001E5 RID: 485
			private Timer timer;
		}
	}
}
