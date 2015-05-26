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
    public class WeightedRequestRouter : RequestRouter
    {
        private IList<int> _weights;
        private static int _appServerIndex;
        private static int _appServerWeightedIndex;
        private static readonly object _serverIndexLock = new object();
        
        public WeightedRequestRouter(List<AppServer> appServers, List<int> weights) : base(appServers)
        {
            _weights = new List<int>(weights);
            _appServerIndex = 0;
            _appServerWeightedIndex = 0;
        }

        override public void RouteRequest(HttpContext requestContext)
        {
            var server = GetNextServer(requestContext);            
            ProcessRequest(requestContext, server);
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
                    //
                    // this loop get's next server from pool based on rotating index
                    // and checks if that server is up otherwise moves on to the next entry
                    //
                    nextServer = AppServers[_appServerIndex];
                    _appServerWeightedIndex++;

                    int currentServerSelectionsLeft = _weights[_appServerIndex] - _appServerWeightedIndex;

                    if (currentServerSelectionsLeft == 0 || nextServer.IsDown)
                    {
                        // move to next server
                        _appServerIndex++;
                        _appServerIndex = _appServerIndex % AppServers.Count;

                        // reset the weight counter
                        _appServerWeightedIndex = 0;

                        // and mark this server as counted
                        serverCount--;
                    }

                } while (nextServer.IsDown && serverCount > 0);
            }

            // did loop return a bad server after exhaustion?
            if (nextServer.IsDown)
            {
                throw new HttpException(503, "No backend server(s) available");
            }

            return nextServer;
        }
    }
}