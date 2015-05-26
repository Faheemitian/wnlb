using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NLBLib.Routers
{
    public class CookieBasedRequestRouter : RequestRouter
    {
        private int _appServerIndex;
        private readonly object _serverIndexLock = new object();
        const string COOKIE_NAME = "WNLB_SERVER";
        const int COOKIE_EXPIRE_HOURS = 24;

        public CookieBasedRequestRouter(List<AppServer> servers)
            : base(servers)
        {
        }

        public override Servers.AppServer GetNextServer(HttpContext requestContext)
        {
            var request = requestContext.Request;
            var response = requestContext.Response;
            AppServer nextServer = null;

            if (request.Cookies[COOKIE_NAME] != null)
            {
                var serverCookie = request.Cookies[COOKIE_NAME];
                string serverName = serverCookie.Value;

                foreach (var server in AppServers)
                {
                    if (server.Name.Equals(serverName, StringComparison.OrdinalIgnoreCase))
                    {
                        nextServer = server;
                        break;
                    }
                }
            }

            if (nextServer == null || nextServer.IsDown) // did we find that server from cookie?
            {
                //
                // if not then assinged next server in round robin fashion
                //
                lock (_serverIndexLock)
                {
                    // Multiple threads will mess up it's values so we have to keep
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
                    throw new HttpException(503, "No backend server available");
                }

                //
                // Now store the selection fro affinity
                //
                HttpCookie serverCookie = new HttpCookie(COOKIE_NAME, nextServer.Name);
                serverCookie.Expires = DateTime.Now.AddHours(COOKIE_EXPIRE_HOURS);
                serverCookie.HttpOnly = true;
                response.Cookies.Add(serverCookie);
            }
            

            return nextServer;
        }

        public override void RouteRequest(System.Web.HttpContext requestContext)
        {
            var server = GetNextServer(requestContext);
            ProcessRequest(requestContext, server);
        }
    }
}
