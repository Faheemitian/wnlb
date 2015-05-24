using NLBLib.Applications;
using NLBLib.Routers;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace NLBLib
{
    /// <summary>
    /// <code>IHttpModule</code> class deritive. This class when registered as HttpModule subscribes
    /// to ASP.NET application <code>BeginRequest</code> and <code>EndRequest</code> events and 
    /// sets up the load balancer.
    /// </summary>
    public class LoadBalancerModule : IHttpModule
    {
        public LoadBalancerModule()
        {            
        }

        public static ApplicationRegister AppRegister { get; set; }
        public static ServerRegister ServerRegister { get; set; }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.beginRequestHandler);
            context.EndRequest += new EventHandler(this.endRequestHandler);
        }

        public static void StartServerMintoring()
        {
            ServerMintoringThread.Instance.StartMonitoring(ServerRegister);
        }

        private void endRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpRequest request = context.Request;            
        }

        
        private void beginRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;
            HttpRequest request = context.Request;
            string requestPath = request.Path;

            Application app = AppRegister.GetApplicationForPath(requestPath);
            if (app == null)
            {
                Trace.WriteLine(String.Format("Invalid requeset. No application registered for path {0}", request.Path));
                throw new HttpException(404, "Path not available");
            }

            app.RequestRouter.RouteRequest(context);
        }

        public void Dispose()
        {
        }
    }
}