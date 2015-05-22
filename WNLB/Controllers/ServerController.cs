using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
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
            if (ModelState.IsValid)
            {
                Server oldObject = db.Servers.Find(server.ServerId);
                db.Entry(server).State = EntityState.Modified;
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}