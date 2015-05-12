using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NLBLib.Routers
{
    /// <summary>
    /// Base interface for request routers. Implemented by <see cref="RoundRobinRequestRouter">Router</see>
    /// </summary>
    public interface RequestRouter
    {
        /// <summary>
        /// Gets readonly list of registered app servers
        /// </summary>
        IList<AppServer> AppServers { get; }

        /// <summary>
        /// Applies routing algorithm and gets the next available server from pool.
        /// </summary>
        /// <param name="requestContext">Client request context</param>
        /// <returns>Avaialble server or null if no servers are available</returns>
        AppServer GetNextServer(HttpContext requestContext);        

        /// <summary>
        /// Uses GetNextServer to determine the best route and uses that route to 
        /// server the request.
        /// </summary>
        /// <param name="requestContext"></param>
        void RouteRequest(HttpContext requestContext);
    }
}
