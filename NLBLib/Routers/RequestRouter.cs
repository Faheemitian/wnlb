using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace NLBLib.Routers
{
    public interface RequestRouter
    {
        IList<AppServer> AppServers { get; }
        AppServer GetNextServer(HttpContext requestContext);        
        void RouteRequest(HttpContext requestContext);
    }
}
