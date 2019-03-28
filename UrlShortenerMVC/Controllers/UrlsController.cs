using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using UrlShortenerMVC.Models;
using UrlShortenerMVC.ViewModels;

namespace UrlShortenerMVC.Controllers
{
    public class UrlsController : Controller
    {
        private Entities db = new Entities();
        private readonly string base62 = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // GET: Urls
        public ActionResult Index()
        {
            return View(db.Urls.ToList());
        }

        // GET: Urls/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Url url = db.Urls.Find(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);
        }

        // GET: Urls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Urls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LongUrl")] UrlViewModel model)
        {
            model.LongUrl = model.LongUrl.Trim();
            if (!Uri.IsWellFormedUriString(model.LongUrl, UriKind.Absolute))
            {
                ModelState.AddModelError("InvalidUrl", "Please enter a valid url");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                do
                {
                    model.Id = Guid.NewGuid().ToString();
                } while (db.Urls.Find(model.Id) != null);

                model.Key = GenerateLongToShortKey();
                var temp = model.Key;
                model.ShortUrl = "http://85.75.21.64:6677/";
                do
                {
                    model.ShortUrl += base62[temp % 62];
                    temp /= base62.Length;
                } while (temp > 0);

                model.CreatedAt = DateTime.Now;
                model.Expires = true;
                model.ExpiresAt = model.CreatedAt.AddMonths(3);
                model.MaxClicks = 10;
                model.CurrentClicks = 0;
                model.IsActive = true;  

                db.Urls.Add(model);
                db.SaveChanges();
                
                return View(new UrlViewModel { LongUrl = model.LongUrl, ShortUrl = model.ShortUrl });
            }

            return View(model);
        }

        // GET: Urls/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Url url = db.Urls.Find(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);
        }

        // POST: Urls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LongUrl,ShortUrl,Key,CreatedAt,Expires,ExpiresAt,MaxClicks,CurrentClicks,IsActive")] Url url)
        {
            if (ModelState.IsValid)
            {
                db.Entry(url).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(url);
        }

        // GET: Urls/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Url url = db.Urls.Find(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);
        }

        // POST: Urls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Url url = db.Urls.Find(id);
            db.Urls.Remove(url);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private int GenerateLongToShortKey()
        {
            var r = new Random();
            int key;

            do
            {
                key = r.Next(0, int.MaxValue);
            } while (db.Urls.Count(u => u.Key == key) > 0);

            return key;
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
