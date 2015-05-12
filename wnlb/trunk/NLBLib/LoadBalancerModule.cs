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
        private static ApplicationRegister _appRegister = new ApplicationRegister();
        private static ServerRegister _serverRegister = new ServerRegister();

        static LoadBalancerModule()
        {
            _serverRegister.AddServer(new BasicAppServer("Srv8003", "localhost", 8003));
            _serverRegister.AddServer(new BasicAppServer("Srv8002", "localhost", 8002));

            ServerMintoringThread.Instance.StartMonitoring(_serverRegister);

            var appServers = new List<AppServer> { _serverRegister.GetServerWithName("Srv8003"), 
                _serverRegister.GetServerWithName("Srv8002") };

            var requestRouter = new RoundRobinRequestRouter(appServers);
            _appRegister.AddAppliction(new StaticApplication("AlpahSampleApp", "/", requestRouter));
        }

        public LoadBalancerModule()
        {            
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(this.beginRequestHandler);
            context.EndRequest += new EventHandler(this.endRequestHandler);
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

            Application app = _appRegister.GetApplicationForPath(request.Path);
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