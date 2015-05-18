using Microsoft.Practices.Unity;
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
using WNLB.Misc;

namespace WNLB
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            StartLoadBalancerModule();
        }

        private void StartLoadBalancerModule()
        {
            IUnityContainer container = WNLB.App_Start.UnityConfig.GetConfiguredContainer();
            INLBService service = container.Resolve<INLBService>("NLBService");

            service.ServerRegister.AddServer(new BasicAppServer("Srv8003", "localhost", 8003));
            service.ServerRegister.AddServer(new BasicAppServer("Srv8002", "localhost", 8002));
            service.ServerRegister.AddServer(new LocalAppServer("WNLB_Console"));

            var appServers = new List<AppServer> { service.ServerRegister.GetServerWithName("Srv8003"), 
                    service.ServerRegister.GetServerWithName("Srv8002") };

            var requestRouter = new RoundRobinRequestRouter(appServers);

            service.AppRegister.AddAppliction(new ConfigApplication("WnlbConsoleApp", "/_config"));
            service.AppRegister.AddAppliction(new StaticApplication("AlpahSampleApp", "/", requestRouter));

            LoadBalancerModule.ServerRegister = service.ServerRegister;
            LoadBalancerModule.AppRegister = service.AppRegister;
            LoadBalancerModule.StartServerMintoring();
        }
    }
}