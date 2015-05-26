using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Misc
{
    public class HitCounter
    {
        readonly object _counterLock = new object();
        HitCompCounter _lastMinCounter = HitCompCounter.WithSecs(10, 50);
        HitCompCounter _lastHourCouter = HitCompCounter.WithMins(1, 60);
        HitCompCounter _lastDayCouter = HitCompCounter.WithHours(0.5f, 48);
        HitCompCounter _lastWeekCouter = HitCompCounter.WithDays(0.25f, 56);

        public int[] LastMinHits
        {
            get
            {
                return _lastMinCounter.RecordedHits;
            }
        }

        internal void RecordHit()
        {
            lock (_counterLock)
            {
                _lastMinCounter.RecordHit();
                _lastHourCouter.RecordHit();
                _lastDayCouter.RecordHit();
                _lastWeekCouter.RecordHit();
            }
        }

        class HitCompCounter
        {
            int _hitCounter;
            int _hitCountTime;
            int _totalHits;
            long _nextCollectionTick;
            LinkedList<int> _hitsCollection = new LinkedList<int>();

            internal static HitCompCounter WithDays(float days, int totalHits)
            {
                return new HitCompCounter((int)(days * 86400), totalHits);
            }

            internal static HitCompCounter WithHours(float hours, int totalHits)
            {
                return new HitCompCounter((int)(hours * 3600), totalHits);
            }

            internal static HitCompCounter WithMins(float mins, int totalHits)
            {
                return new HitCompCounter((int)(mins * 60), totalHits);
            }

            internal static HitCompCounter WithSecs(int secs, int totalHits)
            {
                return new HitCompCounter(secs, totalHits);
            }

            internal HitCompCounter(int hitCountTimeSecs, int totalHits)
            {
                _hitCountTime = hitCountTimeSecs;
                _totalHits = totalHits;
            }

            internal void RecordHit()
            {
                _hitCounter++;
                 long nowTick = DateTime.Now.Ticks;
                 if (nowTick >= _nextCollectionTick)
                 {
                     _nextCollectionTick = nowTick + (_hitCountTime * 10000000);
                     _hitsCollection.AddLast(_hitCounter);

                     if (_hitsCollection.Count() > _totalHits)
                     {
                         _hitsCollection.RemoveFirst();
                     }

                     _hitCounter = 0;
                 }
            }

            internal int[] RecordedHits
            {
                get
                {
                    return _hitsCollection.ToArray();
                }
            }
        }
    }
}
