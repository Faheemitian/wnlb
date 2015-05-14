using NLBLib;
using NLBLib.Applications;
using NLBLib.Routers;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WNLB
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static ApplicationRegister _appRegister = new ApplicationRegister();
        private static ServerRegister _serverRegister = new ServerRegister();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            StartLoadBalancerModule();
        }

        private void StartLoadBalancerModule()
        {
            _serverRegister.AddServer(new BasicAppServer("Srv8003", "localhost", 8003));
            _serverRegister.AddServer(new BasicAppServer("Srv8002", "localhost", 8002));
            _serverRegister.AddServer(new LocalAppServer("WNLB_Console"));

            var appServers = new List<AppServer> { _serverRegister.GetServerWithName("Srv8003"), 
                    _serverRegister.GetServerWithName("Srv8002") };

            var requestRouter = new RoundRobinRequestRouter(appServers);

            _appRegister.AddAppliction(new ConfigApplication("WnlbConsoleApp", "/_config")); 
            _appRegister.AddAppliction(new StaticApplication("AlpahSampleApp", "/", requestRouter));            

            LoadBalancerModule.ServerRegister = _serverRegister;
            LoadBalancerModule.AppRegister = _appRegister;
            LoadBalancerModule.StartServerMintoring();
        }
    }
}
