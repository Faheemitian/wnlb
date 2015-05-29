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
using System.Collections.Concurrent;

namespace NLBLib.Routers
{
    public class RoundRobinRequestRouter : RequestRouter
    {
        private int _appServerIndex;
        private readonly object _serverIndexLock = new object();

        public RoundRobinRequestRouter(List<AppServer> appServers) : base(appServers)
        {
            _appServerIndex = 0;
        }

        override public void RouteRequest(HttpContext requestContext)
        {
            var server = GetNextServer(requestContext);
            if (server != null)
            {
                ProcessRequest(requestContext, server);
            }
        }

        override public AppServer GetNextServer(HttpContext requestContext)
        {
            AppServer nextServer;
            lock (_serverIndexLock)
            {
                // Round-robin depends on shared state _appServerIndex to keep rolling
                // correctly. Multiple threads will mess up it's values so we have to keep
                // this block locked.
                int serverCount = AppServers.Count;

                do
                {
                    // this loop get's next server from pool based on rotating index
                    // and checks if that server is up otherwise moves on to the next entry

                    _appServerIndex = _appServerIndex % AppServers.Count;
                    nextServer = AppServers[_appServerIndex];
                    _appServerIndex++;
                    serverCount--;

                } while (nextServer.IsDown && serverCount > 0);
            }

            // did loop return a bad server after exhaustion?
            if (nextServer.IsDown)
            {
                HttpErrorResponse.Send(requestContext, 503, "No backend server available");
                return null;
            }

            return nextServer;
        }
    }
}