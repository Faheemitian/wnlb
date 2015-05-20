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
            return View(Tuple.Create<IEnumerable<Application>, IEnumerable<Server>>(db.Applications.ToList(), db.Servers.ToList()));
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
