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

        public ConfigAppRequestRouter() : base(new List<AppServer>())
        {
            /* do nothing */
        }

        /// <summary>
        /// Returns context without processing
        /// </summary>
        override public void RouteRequest(HttpContext requestContext)
        {
            /* do nothing */
        }

        /// <summary>
        /// Throws NotImplementedException
        /// </summary>
        override public AppServer GetNextServer(HttpContext requestContext)
        {
            throw new NotImplementedException();
        }
    }
}