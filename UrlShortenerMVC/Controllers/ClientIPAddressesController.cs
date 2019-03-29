using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UrlShortenerMVC.Models;

namespace UrlShortenerMVC.Controllers
{
    public class ClientIPAddressesController : Controller
    {
        private Entities db = new Entities();

        // GET: ClientIPAddresses
        public ActionResult Index()
        {
            return View(db.ClientIPAddresses.ToList());
        }

        // GET: ClientIPAddresses/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientIPAddress clientIPAddress = db.ClientIPAddresses.Find(id);
            if (clientIPAddress == null)
            {
                return HttpNotFound();
            }
            return View(clientIPAddress);
        }

        // GET: ClientIPAddresses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientIPAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,IP,CreatedAt")] ClientIPAddress clientIPAddress)
        {
            if (ModelState.IsValid)
            {
                db.ClientIPAddresses.Add(clientIPAddress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(clientIPAddress);
        }

        // GET: ClientIPAddresses/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientIPAddress clientIPAddress = db.ClientIPAddresses.Find(id);
            if (clientIPAddress == null)
            {
                return HttpNotFound();
            }
            return View(clientIPAddress);
        }

        // POST: ClientIPAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,IP,CreatedAt")] ClientIPAddress clientIPAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientIPAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clientIPAddress);
        }

        // GET: ClientIPAddresses/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ClientIPAddress clientIPAddress = db.ClientIPAddresses.Find(id);
            if (clientIPAddress == null)
            {
                return HttpNotFound();
            }
            return View(clientIPAddress);
        }

        // POST: ClientIPAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ClientIPAddress clientIPAddress = db.ClientIPAddresses.Find(id);
            db.ClientIPAddresses.Remove(clientIPAddress);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
