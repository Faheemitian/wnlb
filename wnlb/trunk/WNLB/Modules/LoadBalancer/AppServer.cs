using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNLB.Modules.LoadBalancer
{
    public interface AppServer
    {
        String Hostname { get; }
        String IPAddress { get; }
        int Port { get; }
        ServerStatus Status { get; }
        int Uptime { get; }
        Boolean HasHeartbeat { get; }
        string Name { get; }
    }
}
