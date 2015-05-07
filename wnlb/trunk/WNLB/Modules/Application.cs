using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WNLB.Modules
{
    interface Application
    {
        public String AppName { get; set; }
        public String AppPath { get; set; }
        public List<AppServer> AppServers { get; set; }
        public RequestRouter RequestRouter { get; set; }
    }
}
