using NLBLib.Applications;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Misc
{
    public interface INLBService
    {
        ApplicationRegister AppRegister { get; set; }
        ServerRegister ServerRegister { get; set; }
    }
}