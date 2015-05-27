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
        HitCompCounter _lastHourCounter = HitCompCounter.WithMins(1, 60);
        HitCompCounter _lastDayCounter = HitCompCounter.WithHours(0.5f, 48);
        HitCompCounter _lastWeekCounter = HitCompCounter.WithDays(0.25f, 56);

        public List<int> LastMinHits
        {
            get
            {
                return _lastMinCounter.RecordedHits;
            }
        }

        public List<int> LastHourHits
        {
            get
            {
                return _lastHourCounter.RecordedHits;
            }
        }

        public List<int> LastDayHits
        {
            get
            {
                return _lastDayCounter.RecordedHits;
            }
        }

        public List<int> LastWeekHits
        {
            get
            {
                return _lastWeekCounter.RecordedHits;
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
                _lastHourCounter.RecordHit();
                _lastDayCounter.RecordHit();
                _lastWeekCounter.RecordHit();
                _lastRecordingTime = DateTime.Now;
            }
        }

        internal void CalculateHits()
        {
            lock (_counterLock)
            {
                _lastMinCounter.CalculateHits();
                _lastHourCounter.CalculateHits();
                _lastDayCounter.CalculateHits();
                _lastWeekCounter.CalculateHits();
                _lastRecordingTime = DateTime.Now;
            }
        }
    }
}
