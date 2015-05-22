using Microsoft.Practices.Unity;
using NLBLib;
using NLBLib.Applications;
using NLBLib.Routers;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WNLB.Misc;
using WNLB.Models;

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

            new AppDbContextInitializer().Init();

            StartLoadBalancerModule();
        }

        private void StartLoadBalancerModule()
        {
            //
            //  Initialize NLBService and the Unity IoC
            //
            IUnityContainer container = WNLB.App_Start.UnityConfig.GetConfiguredContainer();
            INLBService service = container.Resolve<INLBService>("NLBService");

            //
            // Add default _config console app
            //
            service.ServerRegister.AddServer(new LocalAppServer("WNLB_Console"));
            service.AppRegister.AddAppliction(new ConfigApplication("WnlbConsoleApp", "/_config"));

            //
            // Add servers and applications from database
            //
            WNLBContext db = new WNLBContext();
            foreach (Server server in db.Servers.ToList())
            {
                service.AddServer(server);
            }

            foreach (WNLB.Models.Application app in db.Applications.Include(app => app.Servers).ToList())
            {
                service.AddApplication(app);
            }

            //
            // Initalize load balancer and start server monitoring threads
            //
            LoadBalancerModule.ServerRegister = service.ServerRegister;
            LoadBalancerModule.AppRegister = service.AppRegister;
            LoadBalancerModule.StartServerMintoring();
        }
    }
}