using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Modules.LoadBalancer
{
    public class BasicAppServer : AppServer
    {
        private readonly string _hostname;
        private readonly string _ipAddress;
        private readonly int _port;

        public BasicAppServer(string hostname, string ipAddress, int port)
        {
            _hostname = hostname;
            _ipAddress = ipAddress;
            _port = port;
        }

        public int Port
        {
            get { return _port; }
        }

        public String IPAddress
        {
            get { return _ipAddress; }            
        }


        public String Hostname
        {
            get { return _hostname; }            
        }

        public ServerStatus Status { get; private set; }
        public int Uptime { get; private set;  }
        public Boolean HasHeartbeat { get; private set; }

        public override string ToString()
        {
            return Hostname;
        }
    }
}