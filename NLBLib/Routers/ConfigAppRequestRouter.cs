using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using NLBLib.Misc;
using System.Web.Management;
using NLBLib.HealthEvents;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace NLBLib.Routers
{
    /// <summary>
    /// Special router that does nothing and leaves the request for asp.net caller to handle
    /// </summary>
    public class ConfigAppRequestRouter : RequestRouter
    {

        public ConfigAppRequestRouter()
        {
        }

        /// <summary>
        /// Returns context without processing
        /// </summary>
        public void RouteRequest(HttpContext requestContext)
        {            
        }

        /// <summary>
        /// Throws NotImplementedException
        /// </summary>
        public AppServer GetNextServer(HttpContext requestContext)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Throws NotImplementedException
        /// </summary>
        public IList<AppServer> AppServers
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public void RemoveServer(string serverName)
        {
            throw new NotImplementedException();
        }
    }
}