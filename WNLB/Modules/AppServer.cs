using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNLB.Modules
{
    public interface AppServer
    {
        public String Hostname { get; set; }
        public String IPAddress { get; set; }
        public int Port { get; set; }
        public ServerStatus Status { get; set; }
        public int Uptime { get; set; }
        public Boolean HasHeartbeat { get; set; }
    }
}
