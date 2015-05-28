using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WNLB.Misc;
using WNLB.Models;

namespace WNLB.Controllers
{
    public class ServerController : Controller
    {
        private WNLBContext db = new WNLBContext();

        [Dependency("NLBService")]
        public INLBService NLBService { get; set; }

        //
        // GET: /Server/

        public ActionResult Index()
        {
            return View(db.Servers.ToList());
        }

        //
        // GET: /Server/Details/5

        public ActionResult Details(int id = 0)
        {
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }
            return View(server);
        }

        //
        // GET: /Server/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Server/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Server server)
        {
            if (NLBService.ServerRegister.GetServerWithName(server.ServerName) != null)
            {
                ModelState.AddModelError("ServerName", "Another server already registered under this name");
            }

            if (ModelState.IsValid)
            {
                db.Servers.Add(server);
                db.SaveChanges();

                //
                // Add server to load balancer
                //
                NLBService.AddServer(server);

                return RedirectToAction("Index", "Home");
            }

            return View(server);
        }

        //
        // GET: /Server/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }
            return View(server);
        }

        //
        // POST: /Server/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Server server)
        {
            Server oldObject = db.Servers.Find(server.ServerId);

            if (!oldObject.ServerName.Equals(server.ServerName, StringComparison.OrdinalIgnoreCase))
            {
                if (NLBService.ServerRegister.GetServerWithName(server.ServerName) != null)
                {
                    ModelState.AddModelError("ServerName", "Another server already registered under this name");
                }
            }

            if (ModelState.IsValid)
            {
                //db.Entry(server).State = EntityState.Modified;
                db.Entry(oldObject).CurrentValues.SetValues(server);
                db.SaveChanges();

                //
                // Update the server object in LoadBalancer
                //
                NLBService.UpdateServer(oldObject.ServerName, server);

                return RedirectToAction("Index", "Home");
            }

            return View(server);
        }

        //
        // GET: /Server/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Server server = db.Servers.Find(id);
            if (server == null)
            {
                return HttpNotFound();
            }
            return View(server);
        }

        //
        // POST: /Server/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Server server = db.Servers.Find(id);
            db.Servers.Remove(server);
            db.SaveChanges();

            //
            // Remove server object from Load balancer
            // 
            NLBService.RemoveServer(server.ServerName);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult DetailedStats()
        {
            List<ServerStats> stats = new List<ServerStats>();
            foreach (var appServer in NLBService.ServerRegister.Servers)
            {
                if (!appServer.IsConfig) { 
                    ServerStats stat = new ServerStats();
                    stat.Name = appServer.Name;
                    stat.Status = appServer.Status.ToString();
                    stats.Add(stat);
                }
            }

            return View(stats);
        }

        [HttpGet]
        public ActionResult LastMinHitsChart(string serverName)
        {
            if (serverName != null && serverName.Length > 0)
            {
                var server = NLBService.ServerRegister.GetServerWithName(serverName);
                if (server != null)
                {
                    var key = new Chart(width: 350, height: 250, theme: ChartTheme.Vanilla)
                        .AddSeries(chartType: "line", yValues: server.HitCounter.LastMinHits);

                    return File(key.ToWebImage().GetBytes(), "image/jpeg");
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult LastHourHitsChart(string serverName)
        {
            if (serverName != null && serverName.Length > 0)
            {
                var server = NLBService.ServerRegister.GetServerWithName(serverName);
                if (server != null)
                {
                    var key = new Chart(width: 350, height: 250, theme: ChartTheme.Vanilla)
                        .AddSeries(chartType: "line", yValues: server.HitCounter.LastHourHits);

                    return File(key.ToWebImage().GetBytes(), "image/jpeg");
                }
            }

            return null;
        }

        public ActionResult LastDayHitsChart(string serverName)
        {
            if (serverName != null && serverName.Length > 0)
            {
                var server = NLBService.ServerRegister.GetServerWithName(serverName);
                if (server != null)
                {
                    var key = new Chart(width: 350, height: 250, theme: ChartTheme.Vanilla)
                        .AddSeries(chartType: "line", yValues: server.HitCounter.LastDayHits);

                    return File(key.ToWebImage().GetBytes(), "image/jpeg");
                }
            }

            return null;
        }

        public ActionResult LastWeekHitsChart(string serverName)
        {
            if (serverName != null && serverName.Length > 0)
            {
                var server = NLBService.ServerRegister.GetServerWithName(serverName);
                if (server != null)
                {
                    var key = new Chart(width: 350, height: 250, theme: ChartTheme.Vanilla)
                        .AddSeries(chartType: "line", yValues: server.HitCounter.LastWeekHits);

                    return File(key.ToWebImage().GetBytes(), "image/jpeg");
                }
            }

            return null;
        }

        [HttpGet]
        public ActionResult Stats()
        {
            List<ServerStats> stats = new List<ServerStats>();
            foreach (var appServer in NLBService.ServerRegister.Servers)
            {
                ServerStats stat = new ServerStats();
                stat.Name = appServer.Name;
                stat.Status = appServer.Status.ToString();

                List<int> hits = appServer.HitCounter.LastMinHits;
                TimeSpan span = DateTime.Now - appServer.HitCounter.LastRecordingTime;
                int diffs = (int)span.TotalSeconds / 5;
                for (int i = 0; i < diffs && i < 10; i++)
                {
                    hits.Add(0);
                }

                stat.HitsPerMin = hits.ToArray();
                stats.Add(stat);
            }

            return Json(stats.ToArray(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}