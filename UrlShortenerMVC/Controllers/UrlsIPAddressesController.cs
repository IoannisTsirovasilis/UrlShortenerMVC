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
    public class UrlsIPAddressesController : Controller
    {
        private Entities db = new Entities();

        // GET: UrlsIPAddresses
        public ActionResult Index()
        {
            var urlsIPAddresses = db.UrlsIPAddresses.Include(u => u.AspNetUser).Include(u => u.ClientIPAddress).Include(u => u.Url);
            return View(urlsIPAddresses.ToList());
        }

        // GET: UrlsIPAddresses/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlsIPAddress urlsIPAddress = db.UrlsIPAddresses.Find(id);
            if (urlsIPAddress == null)
            {
                return HttpNotFound();
            }
            return View(urlsIPAddress);
        }

        // GET: UrlsIPAddresses/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.IPAddressId = new SelectList(db.ClientIPAddresses, "Id", "IP");
            ViewBag.UrlId = new SelectList(db.Urls, "Id", "LongUrl");
            return View();
        }

        // POST: UrlsIPAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UrlId,IPAddressId,UserId,CreatedAt,ClickedAt")] UrlsIPAddress urlsIPAddress)
        {
            if (ModelState.IsValid)
            {
                db.UrlsIPAddresses.Add(urlsIPAddress);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", urlsIPAddress.UserId);
            ViewBag.IPAddressId = new SelectList(db.ClientIPAddresses, "Id", "IP", urlsIPAddress.IPAddressId);
            ViewBag.UrlId = new SelectList(db.Urls, "Id", "LongUrl", urlsIPAddress.UrlId);
            return View(urlsIPAddress);
        }

        // GET: UrlsIPAddresses/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlsIPAddress urlsIPAddress = db.UrlsIPAddresses.Find(id);
            if (urlsIPAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", urlsIPAddress.UserId);
            ViewBag.IPAddressId = new SelectList(db.ClientIPAddresses, "Id", "IP", urlsIPAddress.IPAddressId);
            ViewBag.UrlId = new SelectList(db.Urls, "Id", "LongUrl", urlsIPAddress.UrlId);
            return View(urlsIPAddress);
        }

        // POST: UrlsIPAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UrlId,IPAddressId,UserId,CreatedAt,ClickedAt")] UrlsIPAddress urlsIPAddress)
        {
            if (ModelState.IsValid)
            {
                db.Entry(urlsIPAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.AspNetUsers, "Id", "Email", urlsIPAddress.UserId);
            ViewBag.IPAddressId = new SelectList(db.ClientIPAddresses, "Id", "IP", urlsIPAddress.IPAddressId);
            ViewBag.UrlId = new SelectList(db.Urls, "Id", "LongUrl", urlsIPAddress.UrlId);
            return View(urlsIPAddress);
        }

        // GET: UrlsIPAddresses/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlsIPAddress urlsIPAddress = db.UrlsIPAddresses.Find(id);
            if (urlsIPAddress == null)
            {
                return HttpNotFound();
            }
            return View(urlsIPAddress);
        }

        // POST: UrlsIPAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            UrlsIPAddress urlsIPAddress = db.UrlsIPAddresses.Find(id);
            db.UrlsIPAddresses.Remove(urlsIPAddress);
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
