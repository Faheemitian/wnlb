using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WNLB.Misc;

namespace WNLB.Controllers
{
    public class HomeController : Controller
    {
        [Dependency("NLBService")]
        public INLBService NLBService { get; set; }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View(NLBService.ServerRegister.GetServerWithName("Srv8003"));
        }
    }
}
