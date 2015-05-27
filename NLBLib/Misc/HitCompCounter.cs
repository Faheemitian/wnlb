using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Misc
{
    internal class HitCompCounter
    {
        int _hitCounter;
        int _hitCountTime;
        int _totalHits;
        long _lastRecordedHitTicks;
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
            _lastRecordedHitTicks = DateTime.Now.Ticks;
        }

        internal void RecordHit()
        {
            _hitCounter++;
            CalculateHits(_hitCounter);
        }

        internal void CalculateHits()
        {
            CalculateHits(_hitCounter);
        }

        private void CalculateHits(int lastHit)
        {
            //
            // calculate how many diffs have we missed
            //
            long nowTick = DateTime.Now.Ticks;
            int diffs = (int)(nowTick - _lastRecordedHitTicks) / (_hitCountTime * 10000000);

            //
            // we don't want too many hits... just what we need
            //
            

            if (diffs > 0)
            {
                _lastRecordedHitTicks = nowTick;
                if (diffs > _totalHits)
                {
                    diffs = _totalHits;
                }

                while (diffs > 1)
                {
                    AddHitsToList(0);
                    diffs--;
                }

                AddHitsToList(_hitCounter);
                _hitCounter = 0;
            }            
        }

        void AddHitsToList(int hits)
        {
            _hitsCollection.AddLast(hits);

            if (_hitsCollection.Count() > _totalHits)
            {
                _hitsCollection.RemoveFirst();
            }
        }

        internal List<int> RecordedHits
        {
            get
            {
                return _hitsCollection.ToList();
            }
        }
    }
}
