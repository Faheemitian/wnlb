using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WNLB.Misc;
using WNLB.Models;

namespace WNLB.Controllers
{
    public class AppController : Controller
    {
        private WNLBContext db = new WNLBContext();

        [Dependency("NLBService")]
        public INLBService NLBService { get; set; }

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
            app.DistributeEvenly = true;
            return View(app);
        }

        //
        // POST: /App/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AppName,Path,AppType,RoutingAlgorithm,DistributeEvenly,Weights")]Application application, string[] servers)
        {
            if (servers != null && servers.Length > 0)
            {
                application.Servers = new List<Server>();
                foreach (string server in servers)
                {
                    Server serverObj = db.Servers.Find(int.Parse(server));
                    application.Servers.Add(serverObj);
                }
            }
            else
            {
                ModelState.AddModelError("Servers", "Select at least one server");
            }

            if (ModelState.IsValid)
            {
                if (application.RoutingAlgorithm == RoutingAlgo.Weighted && application.Servers.Count == 1)
                {
                    application.DistributeEvenly = true;
                }

                db.Applications.Add(application);
                db.SaveChanges();
                NLBService.AddApplication(application);
                return RedirectToAction("Index", "Home");
            }

            SetAppServers(application);
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

        private void UpdateAppServer(string[] servers, Application app)
        {
            var serverSet = new HashSet<string>(servers);
            var appServers = new HashSet<int>(app.Servers.Select(c => c.ServerId));
            foreach (var server in db.Servers)
            {
                if (serverSet.Contains(server.ServerId.ToString()))
                {
                    if (!appServers.Contains(server.ServerId))
                    {
                        app.Servers.Add(server);
                    }
                }
                else
                {
                    if (appServers.Contains(server.ServerId))
                    {
                        app.Servers.Remove(server);
                    }
                }
            }
        }

        //
        // POST: /App/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] servers)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var application = db.Applications
                       .Include(app => app.Servers)
                       .Where(app => app.AppId == id)
                       .Single();

            string oldAppName = application.AppName;

            if (servers == null || servers.Length < 1)
            {
                ModelState.AddModelError("Servers", "Select at least one server");

            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (TryUpdateModel(application, "", new string[] { "AppName", "Path", "AppType", "RoutingAlgorithm", "DistributeEvenly", "Weights" }))
                    {
                        try
                        {
                            UpdateAppServer(servers, application);

                            if (application.RoutingAlgorithm == RoutingAlgo.Weighted && application.Servers.Count == 1)
                            {
                                application.DistributeEvenly = true;
                            }

                            db.SaveChanges();
                            NLBService.UpdateApplication(oldAppName, application);
                            return RedirectToAction("Index", "Home");
                        }
                        catch (Exception /* dex */)
                        {
                            ModelState.AddModelError("", "Unable to save changes.");
                        }

                    }
                }
            }
            
            SetAppServers(application);
            return View(application);
        }

        //
        // GET: /App/Delete/5

        public ActionResult Delete(int id = 0)
        {
            var application = db.Applications
                       .Include(app => app.Servers)
                       .Where(app => app.AppId == id)
                       .Single();

            if (application == null)
            {
                return HttpNotFound();
            }

            SetAppServers(application);
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

            NLBService.RemoveApplication(application.AppName);

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}