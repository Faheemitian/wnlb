using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Modules.LoadBalancer
{
    public class LoadBalancerModule : IHttpModule
    {
        private ApplicationRegister _appRegister = new ApplicationRegister();

        public LoadBalancerModule()
        {
            var appServers = new List<AppServer> { 
                new BasicAppServer("Srv8003", "localhost", "127.0.0.1", 8003),
                new BasicAppServer("Srv8002", "localhost", "127.0.0.1", 8002) 
            };

            var requestRouter = new RoundRobinRequestRouter(appServers);
            _appRegister.AddAppliction(new StaticApplication("AlpahSampleApp", "/", requestRouter));
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
                // TODO: Log this event
                Console.WriteLine("No application registered for path %s", request.Path);
                throw new HttpException(404, "Path not available");
            }

            app.RequestRouter.RouteRequest(context);
        }

        public void Dispose()
        {
        }
    }
}