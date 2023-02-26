using System;
using System.Collections.Generic;

namespace XLua
{
	[LuaCallCSharp]
	public class LuaTimer
	{
		class Timer
		{
			internal int sn;
			internal int cycle;
			internal int deadline;
			internal Func<int, bool> handler;
			internal bool delete;
			internal LinkedList<Timer> container;
		}
		class Wheel
		{
			internal static int dial_scale = 256;
			internal int head;
			internal LinkedList<Timer>[] vecDial;
			internal int dialSize;
			internal int timeRange;
			internal Wheel nextWheel;
			internal Wheel(int dialSize)
			{
				this.dialSize = dialSize;
				this.timeRange = dialSize * dial_scale;
				this.head = 0;
				this.vecDial = new LinkedList<Timer>[dial_scale];
				for (int i = 0; i < dial_scale; ++i)
				{
					this.vecDial[i] = new LinkedList<Timer>();
				}
			}
			internal LinkedList<Timer> nextDial()
			{
				return vecDial[head++];
			}
			internal void add(int delay, Timer tm)
			{
				var container = vecDial[(head + (delay - (dialSize - jiffies_msec)) / dialSize) % dial_scale];
				container.AddLast(tm);
				tm.container = container;
			}
		}
		static int nextSn = 0;
		static int jiffies_msec = 20;
		static float jiffies_sec = jiffies_msec * .001f;
		static Wheel[] wheels;
		static float pileSecs;
		static float nowTime;
		static Dictionary<int, Timer> mapSnTimer;
		static LinkedList<Timer> executeTimers;
		static bool is_pause = false;

		static int intpow(int n, int m)
		{
			int ret = 1;
			for (int i = 0; i < m; ++i)
				ret *= n;
			return ret;
		}

		static void innerAdd(int deadline, Timer tm)
		{
			tm.deadline = deadline;
			int delay = Math.Max(0, deadline - now());
			Wheel suitableWheel = wheels[wheels.Length - 1];
			for (int i = 0; i < wheels.Length; ++i)
			{
				var wheel = wheels[i];
				if (delay < wheel.timeRange)
				{
					suitableWheel = wheel;
					break;
				}
			}
			suitableWheel.add(delay, tm);
		}

		static void innerDel(Timer tm)
		{
			innerDel(tm, true);
		}

		static void innerDel(Timer tm, bool removeFromMap)
		{
			tm.delete = true;
			if (tm.container != null)
			{
				tm.container.Remove(tm);
				tm.container = null;
			}
			if (removeFromMap) mapSnTimer.Remove(tm.sn);
		}

		static int now()
		{
			return (int)(nowTime * 1000);
		}

		internal static void tick(float deltaTime)
		{
			if (is_pause) return;
			nowTime += deltaTime;
			pileSecs += deltaTime;
			int cycle = 0;
			while (pileSecs >= jiffies_sec)
			{
				pileSecs -= jiffies_sec;
				cycle++;
			}
			for (int i = 0; i < cycle; ++i)
			{
				var timers = wheels[0].nextDial();
				LinkedListNode<Timer> node = timers.First;
				for (int j = 0; j < timers.Count; ++j)
				{
					var tm = node.Value;
					executeTimers.AddLast(tm);
					node = node.Next;
				}
				timers.Clear();

				for (int j = 0; j < wheels.Length; ++j)
				{
					var wheel = wheels[j];
					if (wheel.head == Wheel.dial_scale)
					{
						wheel.head = 0;
						if (wheel.nextWheel != null)
						{
							var tms = wheel.nextWheel.nextDial();
							LinkedListNode<Timer> tmsNode = tms.First;
							for (int k = 0; k < tms.Count; ++k)
							{
								var tm = tmsNode.Value;
								if (tm.delete)
								{
									mapSnTimer.Remove(tm.sn);
								}
								else
								{
									innerAdd(tm.deadline, tm);
								}
								tmsNode = tmsNode.Next;
							}
							tms.Clear();
						}
					}
					else
					{
						break;
					}
				}
			}

			while (executeTimers.Count > 0)
			{
				var tm = executeTimers.First.Value;
				executeTimers.Remove(tm);
				if (!tm.delete && tm.handler(tm.sn) && tm.cycle > 0)
				{
					innerAdd(now() + tm.cycle, tm);
				}
				else
				{
					mapSnTimer.Remove(tm.sn);
				}
			}
		}

		internal static void init()
		{
			wheels = new Wheel[4];
			for (int i = 0; i < 4; ++i)
			{
				wheels[i] = new Wheel(jiffies_msec * intpow(Wheel.dial_scale, i));
				if (i > 0)
				{
					wheels[i - 1].nextWheel = wheels[i];
				}
			}
			mapSnTimer = new Dictionary<int, Timer>();
			executeTimers = new LinkedList<Timer>();
		}

		static int fetchSn()
		{
			return ++nextSn;
		}

		public static int Add(int delay, Action<int> handler)
		{
			return Add(delay, 0, (int sn) =>
			{
				handler(sn);
				return false;
			});
		}

		public static int Add(int delay, int cycle, Func<int, bool> handler)
		{
			Timer tm = new Timer();
			tm.sn = fetchSn();
			tm.cycle = cycle;
			tm.handler = handler;
			mapSnTimer[tm.sn] = tm;
			innerAdd(now() + delay, tm);
			return tm.sn;
		}

		public static void ChgCycleTime(int sn, int cycle)
		{
			if (mapSnTimer == null) return;

			Timer tm;
			if (mapSnTimer.TryGetValue(sn, out tm))
			{
				tm.cycle = cycle;
			}
		}

		public static bool IsActive(int sn)
        {
			if (mapSnTimer == null) return false;
			return mapSnTimer.ContainsKey(sn);
		}

		public static void Delete(int sn)
		{
			if (mapSnTimer == null) return;

			Timer tm;
			if (mapSnTimer.TryGetValue(sn, out tm))
			{
				innerDel(tm);
			}
		}
		public static void DeleteAll(IntPtr l)
		{
			if (mapSnTimer == null) return;
			foreach (var t in mapSnTimer)
			{
				innerDel(t.Value, false);
			}
			mapSnTimer.Clear();
		}

		public static void Pause(bool isPause)
        {
			is_pause = isPause;
		}
	}
}
