using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Misc
{

    /// <summary>
    /// Can be used for collections that use case insensitive keys
    /// </summary>
    public class CaseInsensitiveStringComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (x == null && y == null) return true;
            if (x == null && y != null || y == null && x != null) return false;

            return String.Compare(x, y, true) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
