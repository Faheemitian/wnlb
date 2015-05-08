using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WNLB.Modules.LoadBalancer
{
    public enum ServerStatus
    {
        UKNOWN,
        AVAILABLE,
        SWAMPED,
        DOWN
    }
}
