using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WNLB.Modules.LoadBalancer
{
    public class RoundRobinRequestRouter : RequestRouter
    {
        private List<AppServer> _appServers;
        private static int _appServerIndex;
        private HttpClient _client;

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
            String uriString = String.Format("{0}://{1}:{2}{3}", request.Url.Scheme, server.Hostname, server.Port, request.Url.PathAndQuery);
            Uri routedUri = new Uri(uriString);
            HttpRequestMessage forwardRequest = new HttpRequestMessage(method, routedUri);

            HttpResponseMessage forwardedResponse = _client.SendAsync(forwardRequest, HttpCompletionOption.ResponseHeadersRead).Result;
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
            if (_appServers.IsNullOrEmpty())
            {
                throw new InvalidOperationException("No app servers available");
            }

            _appServerIndex = _appServerIndex % _appServers.Count;
            var server = _appServers[_appServerIndex];
            _appServerIndex++;

            return server;
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