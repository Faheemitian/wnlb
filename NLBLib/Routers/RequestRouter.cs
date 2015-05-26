using NLBLib.Misc;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace NLBLib.Routers
{
    /// <summary>
    /// Base interface for request routers. Implemented by <see cref="RoundRobinRequestRouter">Router</see>
    /// </summary>
    public abstract class RequestRouter
    {
        private ImmutableList<AppServer> _appServers;
        private HttpRequestProcessor _requestProcessor;

        public RequestRouter(List<AppServer> servers)
        {
            _appServers = ImmutableList.Create<AppServer>(servers.ToArray());
            _requestProcessor = new HttpRequestProcessor();
        }

        /// <summary>
        /// Applies routing algorithm and gets the next available server from pool.
        /// </summary>
        /// <param name="requestContext">Client request context</param>
        /// <returns>Avaialble server or null if no servers are available</returns>
        public abstract AppServer GetNextServer(HttpContext requestContext);        

        /// <summary>
        /// Uses GetNextServer to determine the best route and uses that route to 
        /// server the request.
        /// </summary>
        /// <param name="requestContext"></param>
        public abstract void RouteRequest(HttpContext requestContext);

        /// <summary>
        /// Returns readonly collection of registered app servers
        /// </summary>
        public IList<AppServer> AppServers
        {
            get
            {
                return _appServers;
            }
        }

        /// <summary>
        /// Remove given server from app processing
        /// </summary>
        /// <param name="serverName">Remove server</param>
        virtual public AppServer RemoveServer(string serverName)
        {
            //
            // Lock since we are allowing hot edits
            //
            foreach (AppServer server in _appServers)
            {
                if (server.Name.Equals(serverName, StringComparison.OrdinalIgnoreCase))
                {
                    _appServers = _appServers.Remove(server);
                    return server;
                }
            }

            return null;
        }

        // TODO: Implement AddServer function

        protected void ProcessRequest(HttpContext requestContext, AppServer server)
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

            response.Headers.Add("WNLB-Server", server.Name);
            response.End();
        }
    }
}
