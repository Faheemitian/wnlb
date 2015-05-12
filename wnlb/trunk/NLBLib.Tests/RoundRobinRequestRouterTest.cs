using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLBLib.Routers;
using NLBLib.Servers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLBLib.Tests
{
    [TestClass]
    public class RoundRobinRequestRouterTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetNextServerExceptionTest()
        {
            RoundRobinRequestRouter router = new RoundRobinRequestRouter(null);
            router.RouteRequest(null);
        }

        [TestMethod]
        public void GetNextServerTest()
        {
            var appServers = new List<AppServer> {
                new BasicAppServer("Server1", "Hostname", 80),
                new BasicAppServer("Server2", "Hostname", 80),
                new BasicAppServer("Server3", "Hostname", 80)
            };
            RoundRobinRequestRouter router = new RoundRobinRequestRouter(appServers);

            Assert.AreSame("Server1", router.GetNextServer(null).Name);
            Assert.AreSame("Server2", router.GetNextServer(null).Name);
            Assert.AreSame("Server3", router.GetNextServer(null).Name);
            Assert.AreSame("Server1", router.GetNextServer(null).Name);
            Assert.AreSame("Server2", router.GetNextServer(null).Name);
        }
    }
}
