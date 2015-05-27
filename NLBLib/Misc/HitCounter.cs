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
        DateTime _lastRecordingTime = DateTime.Now;
        HitCompCounter _lastMinCounter = HitCompCounter.WithSecs(5, 50);
        HitCompCounter _lastHourCouter = HitCompCounter.WithMins(1, 60);
        HitCompCounter _lastDayCouter = HitCompCounter.WithHours(0.5f, 48);
        HitCompCounter _lastWeekCouter = HitCompCounter.WithDays(0.25f, 56);

        public List<int> LastMinHits
        {
            get
            {
                return _lastMinCounter.RecordedHits;
            }
        }

        public DateTime LastRecordingTime
        {
            get
            {
                return _lastRecordingTime;
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
                _lastRecordingTime = DateTime.Now;
            }
        }

        internal void CalculateHits()
        {
            lock (_counterLock)
            {
                _lastMinCounter.CalculateHits();
                _lastHourCouter.CalculateHits();
                _lastDayCouter.CalculateHits();
                _lastWeekCouter.CalculateHits();
                _lastRecordingTime = DateTime.Now;
            }
        }
    }
}
