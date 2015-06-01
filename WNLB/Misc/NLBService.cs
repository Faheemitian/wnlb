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
            List<NLBLib.Servers.AppServer> appServers = new List<NLBLib.Servers.AppServer>();
            foreach (var server in modelApp.Servers)
            {
                var appServer = ServerRegister.GetServerWithName(server.ServerName);
                appServer.HitCounter.ResetTotalHits();
                appServers.Add(appServer);
            }

            //
            // Select app type and routing algo
            //
            if (modelApp.AppType == ApplicationType.Static)
            {
                RequestRouter router = null;
                switch (modelApp.RoutingAlgorithm)
                {
                   case RoutingAlgo.Weighted:
                        List<int> weightsList = new List<int>();
                        if (modelApp.DistributeEvenly)
                        {
                            for (int i = 0; i < appServers.Count; i++)
                            {
                                weightsList.Add(1);
                            }
                        }
                        else
                        {
                            //
                            // compute weights from comma-seperated list
                            //
                            string[] weightsStrArray = modelApp.Weights.Split(',');
                            int weightIndex = 0;
                            for (int i = 0; i < appServers.Count; i++)
                            {
                                try
                                {
                                    weightsList.Add(Int32.Parse(weightsStrArray[i]));
                                }
                                catch
                                {
                                    // in case of error just ignore the extra weight
                                    weightsList.Add(1);
                                }

                                weightIndex++;

                                // 
                                // rotate weightage if weights are less then selected servers
                                //
                                weightIndex = weightIndex % weightsStrArray.Length;
                            }
                        }
                        router = new WeightedRequestRouter(appServers, weightsList);
                        break;

                    case RoutingAlgo.IPHash:
                        router = new IPHashRequestRouter(appServers);
                        break;

                    case RoutingAlgo.CookieBased:
                        router = new CookieBasedRequestRouter(appServers);
                        break;

                    case RoutingAlgo.RoundRobin:
                    default:
                        router = new RoundRobinRequestRouter(appServers);
                        break;
                }

                AppRegister.AddAppliction(new BasicApplication(modelApp.AppName, modelApp.Path, router));
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