using NLBLib.Applications;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Misc
{
    public class NLBService : INLBService
    {
        public NLBService()
        {
            AppRegister = new ApplicationRegister();
            ServerRegister = new ServerRegister();
        }

        public ApplicationRegister AppRegister { get; set; }
        public ServerRegister ServerRegister { get; set; }
    }
}