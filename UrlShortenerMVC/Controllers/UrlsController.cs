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
            if (ModelState.IsValid)
            {
                do
                {
                    model.Id = Guid.NewGuid().ToString();
                } while (db.Urls.Find(model.Id) != null);

                model.Token = GenerateLongToShortToken();
                var temp = model.Token;
                model.ShortUrl = "http://192.168.1.2:6677/";
                do
                {
                    model.ShortUrl += base62[temp % 62];
                    temp /= base62.Length;
                } while (temp > 0);

                model.CreatedAt = DateTime.Now;

                db.Urls.Add(model);
                db.SaveChanges();
                
                return View(new UrlViewModel { LongUrl = model.LongUrl, ShortUrl = model.ShortUrl });
            }

            return View(model);
        }

        private int GenerateLongToShortToken()
        {
            var r = new Random();
            int token;

            do
            {
                token = r.Next(0, int.MaxValue);
            } while (db.Urls.Count(u => u.Token == token) > 0);

            return token;
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
