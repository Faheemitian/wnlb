using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WNLB.Models;

namespace WNLB.Controllers
{
    public class AppController : Controller
    {
        private WNLBContext db = new WNLBContext();

        //
        // GET: /App/

        public ActionResult Index()
        {
            return View(db.Applications.ToList());
        }

        //
        // GET: /App/Details/5

        public ActionResult Details(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        //
        // GET: /App/Create

        public ActionResult Create()
        {
            var app = new Application();
            SetAppServers(app);
            return View(app);
        }

        //
        // POST: /App/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppName,Path,AppType,RoutingAlgorithm")]Application application, string[] servers)
        {
            if (servers != null)
            {
                application.Servers = new List<Server>();
                foreach (string server in servers)
                {
                    Server serverObj = db.Servers.Find(int.Parse(server));
                    application.Servers.Add(serverObj);
                }
            }

            if (ModelState.IsValid)
            {
                db.Applications.Add(application);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(application);
        }

        //
        // GET: /App/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }

            SetAppServers(application);
            return View(application);
        }

        private void SetAppServers(Application app)
        {
            var servers = db.Servers;
            var appServers = new HashSet<int>(app.Servers.Select(s => s.ServerId));
            var viewModel = new List<AppServer>();
            foreach (var server in servers)
            {
                viewModel.Add(new AppServer()
                {
                    ServerId = server.ServerId,
                    ServerName = server.ServerName,
                    IsSelected = appServers.Contains(server.ServerId)
                });
            }
            ViewBag.Servers = viewModel;
        }

        //
        // POST: /App/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Application application)
        {
            if (ModelState.IsValid)
            {
                db.Entry(application).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(application);
        }

        //
        // GET: /App/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Application application = db.Applications.Find(id);
            if (application == null)
            {
                return HttpNotFound();
            }
            return View(application);
        }

        //
        // POST: /App/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Application application = db.Applications.Find(id);
            db.Applications.Remove(application);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}