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

namespace NLBLib.Routers
{
    public class IPHashRequestRouter : RequestRouter
    {
        private IList<AppServer> _appServers;
        private static int _appServerIndex;
        private HttpRequestProcessor _requestProcessor;
        private static readonly object _serverIndexLocker = new object();

        public IPHashRequestRouter(List<AppServer> appServers)
        {
            _appServers = new List<AppServer>(appServers);
            _appServerIndex = 0;
            _requestProcessor = new HttpRequestProcessor();
        }

        public void RouteRequest(HttpContext requestContext)
        {
            var server = GetNextServer(requestContext);
            ProcessRequest(requestContext, server);
        }

        private void ProcessRequest(HttpContext requestContext, AppServer server)
        {
            HttpRequest request = requestContext.Request;
            HttpResponse response = requestContext.Response;

            try
            {
                _requestProcessor.ProcessSendRequest(server, request, response);

            }
            catch (SocketException)
            {
                // server is down
                server.Status = ServerStatus.DOWN;

                // re-route request
                RouteRequest(requestContext);
                return;
            }
            catch (Exception ex)
            {
                //
                // If something else is wrong, just forward it
                //
                Debug.Write("Failed to process request: " + ex.Message);
                throw new HttpException(500, "Server Error");
            }

            //response.Headers.Add("X-Server", server.Name);
            response.End();
        }

        public AppServer GetNextServer(HttpContext requestContext)
        {
            AppServer nextServer;
            lock (_serverIndexLocker)
            {
                // Round-robin depends on shared state _appServerIndex to keep rolling
                // correctly. Multiple threads will mess up it's values so we have to keep
                // this block locked.
                int serverCount = _appServers.Count;

                do
                {
                    // this loop get's next server from pool based on rotating index
                    // and checks if that server is up otherwise moves on to the next entry

                    _appServerIndex = _appServerIndex % _appServers.Count;
                    nextServer = _appServers[_appServerIndex];
                    _appServerIndex++;
                    serverCount--;

                } while (nextServer.IsDown && serverCount > 0);
            }

            // did loop return a bad server after exhaustion?
            if (nextServer.IsDown)
            {
                throw new HttpException(503, "No backend server available");
            }

            return nextServer;
        }

        /// <inheritdoc />
        public void RemoveServer(string serverName)
        {
            //
            // Lock since we are allowing hot edits
            //
            lock (_serverIndexLocker)
            {
                AppServer theServer = null;
                foreach (AppServer server in _appServers)
                {
                    if (server.Name.Equals(serverName, StringComparison.OrdinalIgnoreCase))
                    {
                        theServer = server;
                        break;
                    }
                }

                if (theServer != null)
                {
                    _appServers.Remove(theServer);
                }
            }
        }

        /// <inheritdoc />
        public IList<AppServer> AppServers
        {
            get
            {
                return new ReadOnlyCollection<AppServer>(_appServers);
            }
        }
    }
}