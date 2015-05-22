using NLBLib.Applications;
using NLBLib.Routers;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WNLB.Models;

namespace WNLB.Misc
{
    public class NLBService : INLBService
    {
        public NLBService()
        {
            AppRegister = new ApplicationRegister();
            ServerRegister = new ServerRegister();
        }

        public ApplicationRegister AppRegister { get; set; }
        public ServerRegister ServerRegister { get; set; }

        public void AddServer(Models.Server server)
        {
            ServerRegister.AddServer(new BasicAppServer(server.ServerName, server.ServerHost, server.Port));
        }

        public void UpdateServer(string serverName, Models.Server server)
        {
            //
            // Removes the old server entry and then adds new server entry
            //
            ServerRegister.RemoveServer(serverName);
            AddServer(server);
        }

        public void RemoveServer(string serverName)
        {
            ServerRegister.RemoveServer(serverName);
            foreach(var app in AppRegister.Applications) {
                app.RequestRouter.RemoveServer(serverName);
            }
        }

        public void AddApplication(Models.Application modelApp)
        {
            //
            // Load servers assigned to the app
            //
            List<NLBLib.Servers.AppServer> appServes = new List<NLBLib.Servers.AppServer>();
            foreach (var server in modelApp.Servers)
            {
                appServes.Add(ServerRegister.GetServerWithName(server.ServerName));
            }

            //
            // Select app type and routing algo
            //
            if (modelApp.AppType == ApplicationType.Static)
            {
                RequestRouter router = null;
                switch (modelApp.RoutingAlgorithm)
                {
                    case RoutingAlgo.RoundRobin:
                        router = new RoundRobinRequestRouter(appServes);
                        break;
                    case RoutingAlgo.Wighted:
                        break;
                    case RoutingAlgo.IPHash:
                        break;
                    default:
                        router = new RoundRobinRequestRouter(appServes);
                        break;
                }

                AppRegister.AddAppliction(new StaticApplication(modelApp.AppName, modelApp.Path, router));
            }
        }

        public void UpdateApplication(string oldName, Models.Application modelApp)
        {
            //
            // Remove and then add app in the list
            // 
            RemoveApplication(oldName);

            AddApplication(modelApp);
        }

        public void RemoveApplication(string appName)
        {
            AppRegister.RemoveApplication(appName);
        }
    }
}