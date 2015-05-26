using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Misc
{
    class ConsistentHash<T>
    {
        SortedDictionary<int, T> _circle = new SortedDictionary<int, T>();
        int _replicate = 160;   // default _replicate count
        int[] _circleKeys = null;    //cache the ordered keys for better performance
        MurmurHash2UInt32Hack _murmur = new MurmurHash2UInt32Hack();

        public void Init(IEnumerable<T> nodes)
        {
            Init(nodes, _replicate);
        }

        public void Init(IEnumerable<T> nodes, int replicate)
        {
            _replicate = replicate;

            foreach (T node in nodes)
            {
                this.Add(node, false);
            }
            _circleKeys = _circle.Keys.ToArray();
        }

        public void Add(T node)
        {
            Add(node, true);
        }

        private void Add(T node, bool updateKeyArray)
        {
            for (int i = 0; i < _replicate; i++)
            {
                int hash = BetterHash(node.GetHashCode().ToString() + i);
                _circle[hash] = node;
            }

            if (updateKeyArray)
            {
                _circleKeys = _circle.Keys.ToArray();
            }
        }

        public void Remove(T node)
        {
            for (int i = 0; i < _replicate; i++)
            {
                int hash = BetterHash(node.GetHashCode().ToString() + i);
                if (_circle.Remove(hash))
                {
                    _circleKeys = _circle.Keys.ToArray();
                }
            }
        }

        //return the index of first item that >= val.
        //if not exist, return 0;
        //ay should be ordered array.
        int First_ge(int[] ay, int val)
        {
            int begin = 0;
            int end = ay.Length - 1;

            if (ay[end] < val || ay[0] > val)
            {
                return 0;
            }

            int mid = begin;
            while (end - begin > 1)
            {
                mid = (end + begin) / 2;
                if (ay[mid] >= val)
                {
                    end = mid;
                }
                else
                {
                    begin = mid;
                }
            }

            return end;
        }

        public T GetNode(String key)
        {
            int hash = BetterHash(key);

            int first = First_ge(_circleKeys, hash);

            return _circle[_circleKeys[first]];
        }

        //default String.GetHashCode() can't well spread strings like "1", "2", "3"
        public int BetterHash(String key)
        {
            uint hash = _murmur.Hash(Encoding.ASCII.GetBytes(key));
            return (int)hash;
        }
    }
}
