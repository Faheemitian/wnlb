using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NLBLib.Servers
{
    public class BasicAppServer : AppServer
    {
        public BasicAppServer(string name, string host, int port) : base(name, host, port)
        {
        }        
    }
}