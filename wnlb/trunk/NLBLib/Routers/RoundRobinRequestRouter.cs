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
    public class RoundRobinRequestRouter : RequestRouter
    {
        private List<AppServer> _appServers;
        private static int _appServerIndex;
        private HttpClient _client;
        private static readonly object _serverIndexLocker = new object();

        public RoundRobinRequestRouter(List<AppServer> appServers)
        {
            _appServers = appServers;
            _appServerIndex = 0;
            _client = new HttpClient();
        }

        public void RouteRequest(HttpContext requestContext)
        {
            var server = GetNextServer(requestContext);            
            ProcessRequest(requestContext, server);
        }

        private void ProcessRequest(HttpContext requestContext, AppServer server)
        {
            HttpRequest request = requestContext.Request;
            HttpMethod method = new HttpMethod(request.HttpMethod);
            String uriString = String.Format("{0}://{1}:{2}{3}", request.Url.Scheme, server.Host, server.Port, request.Url.PathAndQuery);
            Uri routedUri = new Uri(uriString);
            HttpRequestMessage forwardRequest = new HttpRequestMessage(method, routedUri);

            HttpResponseMessage forwardedResponse = null;

            try
            {
                forwardedResponse = _client.SendAsync(forwardRequest, HttpCompletionOption.ResponseHeadersRead).Result;

            } 
            catch(AggregateException ae)
            {
                Exception ex = ae.InnerException;
                while (ex.InnerException != null) ex = ex.InnerException;
                if (ex is SocketException)
                {
                    // server is down
                    server.Status = ServerStatus.DOWN;

                    // re-route request
                    RouteRequest(requestContext);
                    return;
                }
                else
                {
                    Debug.Write("Failed to process request " + ex.Message);
                    throw new HttpException(500, "Server Error");
                }
            }

            HttpResponse response = requestContext.Response;

            foreach (var header in forwardedResponse.Headers)
            {
                StringBuilder headerValue = new StringBuilder();
                foreach (var value in header.Value)
                {
                    headerValue.Append(value).Append(";");
                }

                response.Headers.Add(header.Key, headerValue.ToString());
            }

            response.Headers.Add("X-Server", server.Name);
            var stream = forwardedResponse.Content.ReadAsStreamAsync().Result;
            stream.CopyTo(response.OutputStream);

            response.End();
        }

        public AppServer GetNextServer(HttpContext requestContext)
        {
            AppServer nextServer;
            int serverCount = _appServers.Count;

            lock (_serverIndexLocker)
            {
                // Round-robin depends on shared state _appServerIndex to keep rolling
                // correctly. Multiple threads will mess up it's values so we have to keep
                // this block locked.

                do
                {
                    // this loop get's next server from pool based on rotating index
                    // and checks if that server is up otherwise moves on to the next entry

                    _appServerIndex = _appServerIndex % _appServers.Count;
                    nextServer = _appServers[_appServerIndex];
                    _appServerIndex++;
                    serverCount--;

                } while (!nextServer.IsAvailable && serverCount > 0);
            }

            // did loop return a bad server after exhaustion?
            if (nextServer.IsDown)
            {
                throw new HttpException(503, "No backend server available");
            }

            return nextServer;
        }


        public IList<AppServer> AppServers
        {
            get
            {
                return new ReadOnlyCollection<AppServer>(_appServers);
            }
        }
    }
}