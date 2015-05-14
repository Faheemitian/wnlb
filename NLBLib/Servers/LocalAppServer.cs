using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLBLib.Servers
{
    public class LocalAppServer : AppServer
    {
        public LocalAppServer(string serverName)
            : base("WNLB_Console", "localhost", 0)
        {
        }        
    }
}