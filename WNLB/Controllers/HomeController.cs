using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WNLB.Misc;
using WNLB.Models;

namespace WNLB.Controllers
{
    public class HomeController : Controller
    {
        [Dependency("NLBService")]
        public INLBService NLBService { get; set; }

        private WNLBContext db = new WNLBContext();

        [Authorize]
        public ActionResult Index()
        {
            var apps = db.Applications;
            var appsList = apps.ToList();

            var servers = db.Servers;
            var serverList = servers.ToList();

            foreach (var dbServer in serverList)
            {
                var serviceServer = NLBService.ServerRegister.GetServerWithName(dbServer.ServerName);
                if(serviceServer != null)
                {
                    dbServer.Status = serviceServer.Status.ToString();
                }
            }

            return View(Tuple.Create<IEnumerable<Application>, IEnumerable<Server>>(appsList, serverList));
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
