using NLBLib.Applications;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WNLB.Misc
{
    public interface INLBService
    {
        ApplicationRegister AppRegister { get; set; }
        ServerRegister ServerRegister { get; set; }
        
        /// <summary>
        /// Adds server to server pool in LoadBlancer
        /// </summary>
        /// <param name="server">Server to add</param>
        void AddServer(Models.Server server);

        /// <summary>
        /// Updates server in server pool of LoadBlancer
        /// </summary>
        /// <param name="serverName">Old server name</param>
        /// <param name="server">Updated server object</param>
        void UpdateServer(string serverName, Models.Server server);

        /// <summary>
        /// Removes server from available server pool for LoadBalancing
        /// </summary>
        /// <param name="serverName"></param>
        void RemoveServer(string serverName);

        /// <summary>
        /// Adds application to load balancer
        /// </summary>
        /// <param name="modelApp"></param>
        void AddApplication(Models.Application modelApp);

        /// <summary>
        /// Updates the application object in LoadBalancer
        /// </summary>
        /// <param name="modelApp">Updated app instance</param>
        void UpdateApplication(string oldName, Models.Application modelApp);

        /// <summary>
        /// Removes application from LoadBalancer
        /// </summary>
        /// <param name="appName"></param>
        void RemoveApplication(string appName);
    }
}