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
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections.Immutable;
using System.Threading;

namespace NLBLib.Routers
{
    public class IPHashRequestRouter : RequestRouter
    {
        ConsistentHash<AppServer> _appServerMap = new ConsistentHash<AppServer>();
        private const int _serverDownTimeout = 10; // secs
        private ImmutableList<DelistedServer> _delistedServers = ImmutableList<DelistedServer>.Empty;
        
        public IPHashRequestRouter(List<AppServer> appServers)
            : base(appServers)
        {
            foreach(var server in appServers) {
                _appServerMap.Add(server);
            }
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
            HttpRequest request = requestContext.Request;
            string address = GetIPAddress(request);
            AppServer server = null;
            int serverCount = AppServers.Count();

            CheckDelistedServers();
            
            if (address != null)
            {
                do
                {
                    server = _appServerMap.GetNode(address);
                    if (server.IsDown)
                    {
                        //
                        // Rehash circle mapping 
                        //
                        _appServerMap.Remove(server);
                        serverCount--;
                        _delistedServers = _delistedServers.Add(new DelistedServer(server, DateTime.Now));
                    }
                } while (server.IsDown && serverCount > 0);
            }

            //
            // Make sure server is available or we run out of list
            //

            if (server.IsDown)
            {
                HttpErrorResponse.Send(requestContext, 503, "No backend server available");
                return null;
            }

            return server;
        }

        public override AppServer RemoveServer(string serverName)
        {
            AppServer server = base.RemoveServer(serverName);
            if (server != null)
            {
                _appServerMap.Remove(server);
            }

            return server;
        }

        //
        // Checks if any delisted servers have come back up
        //
        private void CheckDelistedServers()
        {
            //
            // Do not block if another thread is already looking
            //
            if (Monitor.TryEnter(_delistedServers))
            {
                try
                {
                    foreach (var entry in _delistedServers)
                    {
                        if (DateTime.Now >= entry.Time.AddSeconds(_serverDownTimeout) && entry.Server.IsAvailable)
                        {
                            _appServerMap.Add(entry.Server);
                            _delistedServers = _delistedServers.Remove(entry);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(_delistedServers);
                }
            }
        }

        private string GetIPAddress(HttpRequest request)
        {
            string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return request.ServerVariables["REMOTE_ADDR"];
        }

        class DelistedServer
        {
            internal AppServer Server { get; set; }
            internal DateTime Time { get; set; }
            internal DelistedServer(AppServer server, DateTime time) {
                Server = server;
                Time = time;
            }
        }
    }
}